//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionSnapshotProcessor.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   07-Feb-2019
// Description          :   Done changes Alert Exception.
//----------------------------------------------------------------------------
using System.Globalization;
using System.Linq;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using BBS.TracerX;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Snapshots.State;
    using Idera.SQLdm.Common.Snapshots.AlwaysOn;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    //using Idera.SQLdm.Common.SQLsafe;
    using Vim25Api;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.CollectionService.Helpers;
    using System.Data.SqlClient;
    using Idera.SQLdm.Common.Services; // SQLdm 9.0 (Ankit Srivastava) - Grooming Time out -- Added namespace for calling management service
    using System.Data;
    using Common.Snapshots.Cloud;
    using Probes.Collectors;

    /// <summary>
    /// 	Processes collected snapshots, new and previous state information to determine
    /// 	what events need to be generated.
    /// </summary>
    internal class ScheduledCollectionEventProcessor
    {
        private const string CRLF = "\r\n";
        private const string TABLE_VALUE_FMT = "    {0} is {1:F2}% fragmented." + CRLF;
        private const string MULTITABLE_ALERT_VALUE_FMT = "There {4} {0} table{5} in [{2}] that {4} {3} {1} percent fragmented." + CRLF;

        private const string WINDOWS_NET = "windows.net";
        private const string AZURE_COM = "azure.com";
        private const string AMAZONAWS_COM = "amazonaws.com";


        #region fields

        private readonly Logger LOG;
        private readonly AlertableSnapshot alertableRefresh;
        private readonly MonitoredObjectName serverName;
        private readonly MonitoredObjectStateGraph stateGraph;
        private readonly IDictionary<int, List<MetricThresholdEntry>> thresholdInstances;
        private readonly IDictionary<int, bool> metricCompatibilityForSqlExpress; //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
        private readonly bool isServerSqlExpress;  //SQLdm 8.6 -- (Ankit Srivastava) -- if the monitored server is SQL Express
        private readonly bool isFullTextInstalled; //SQLdm 8.6 -- (Ankit Srivastava) -- if the Full Text feature is installed with the current version or not
        private readonly string productEdition; //SQLdm 8.6 -- (Ankit Srivastava) -- SQL Express feature - product edition fo the instance being monitored
        private readonly DateTime timestamp;
        private readonly ScheduledRefreshValueAdapter valueAdapter;
        private readonly MonitoredServerWorkload workload;
        private List<Metric> attemptedMetrics;
        private bool isServerAccessible;

        private MetricThresholdEntry mirrorCommitOverheadThreshold;

        private MetricThresholdEntry mirrorSessionNonPreferredConfigThreshold;
        private MetricThresholdEntry mirrorSessionRoleChangeThreshold;
        private MetricThresholdEntry mirrorSessionStatusThreshold;
        private MetricThresholdEntry mirrorWitnessConnectionThreshold;
        private MetricThresholdEntry unrestoredLogThreshold;
        private MetricThresholdEntry unsentLogThreshold;
        private MetricThresholdEntry unsentTransactionThreshold;

        private delegate IComparable GetRefreshValue(Metric metric, out double? collectedValue, bool isBaselineEnabled = false);//SQLdm 10.0 (Tarun Sapra)- Added a default parameter for baselinedThreshold flag

        //SQLdm10.1 (Srishti Purohit) --the actual calculated metric value instead of calculated percentage value of baseline
        private double? collectedValue = null;
        #endregion

        #region constructors

        internal ScheduledCollectionEventProcessor(AlertableSnapshot refresh, MonitoredServerWorkload workload)
        {
            this.alertableRefresh = refresh;
            this.workload = workload;

            LOG = Logger.GetLogger("ScheduledCollectionEventProcessor-" + refresh.ServerName);

            DateTime? serverTime = refresh.TimeStamp;
            timestamp = serverTime.HasValue ? serverTime.Value : DateTime.Now.ToUniversalTime();
            serverName = new MonitoredObjectName(refresh.ServerName);
            valueAdapter = new ScheduledRefreshValueAdapter(refresh, workload.BaselineMetricMeanCollection);

            //thresholds = workload.Thresholds;
            thresholdInstances = workload.ThresholdInstances;
            stateGraph = workload.StateGraph;
            isServerAccessible = true;
            //SQLdm 8.6 -- (Ankit Srivastava) -SQL Express Monitoring feature -starts
            //Sqldm 10.3 -- (Manali H) - 28410 fix
            productEdition = refresh.ProductEdition;
            isFullTextInstalled = refresh.IsFullTextInstalled;
            //Sqldm 10.3 -- (Manali H) - 28410 fix
            isServerSqlExpress = IsServerSqlExpress();
            // SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
            metricCompatibilityForSqlExpress = workload.MetricCompatibilityForSqlExpress;
            //SQLdm 8.6 -- (Ankit Srivastava) -SQL Express Monitoring feature - ends
        }

        #endregion

        private void ClearEvents(MonitoredObjectStateGraph stateGraph, List<int> excludedMetricIDs)
        {
            foreach (MetricState deviation in stateGraph.GetEvents())
            {
                if (excludedMetricIDs == null || !excludedMetricIDs.Contains(deviation.MetricID))
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        // Checks the Default Threshold Entry
        private bool IsMetricEnabled(Metric metric)
        {
            return IsMetricEnabled((int)metric);
        }

        private bool IsMetricEnabled(int metric)
        {
            // Retrieve the state of the metric itself from the Default Instance
            var mte = GetThresholdEntry(metric, String.Empty);

            if (mte == null)
            {
                LOG.DebugFormat("Could not find determine the state for metric [{0}].", metric);
            }

            return mte != null && mte.IsEnabled;
        }

        private bool IsThresholdEnabled(Metric metric)
        {
            return IsThresholdEnabled((int)metric);
        }

        // Checks the Default Threshold Entry
        private bool IsThresholdEnabled(int metric)
        {
            return IsThresholdEnabled(metric, String.Empty);
        }

        // VH - Removed 6/2012 - Not used?
        //private bool IsThresholdEnabled(Metric metric, string thresholdInstance)
        //{
        //    return IsThresholdEnabled((int) metric, thresholdInstance);
        //}

        private bool IsThresholdEnabled(int metric, string thresholdInstance)
        {
            var mte = GetThresholdEntry(metric, thresholdInstance);
            if (mte == null)
            {
                LOG.DebugFormat("Could not find any threshold entries for metric [{0}] and instance [{1}].", metric, thresholdInstance);
            }

            return mte != null && mte.IsThresholdEnabled;
        }

        private void ClearEvents(Metric metric)
        {
            ClearEvents((int)metric);
        }

        private void ClearEvents(int metricID)
        {
            foreach (MetricState deviation in stateGraph.GetEvents())
            {
                if (deviation.MetricID == metricID)
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        private void ClearEvents(Metric metric, string resourceName, InstanceType resourceType)
        {
            ClearEvents((int)metric, resourceName, resourceType);
        }

        private void ClearEvents(int metric, string resourceName, InstanceType resourceType)
        {
            foreach (MetricState deviation in stateGraph.GetEvents())
            {
                if ((deviation.MetricID == metric) &&
                    (deviation.MonitoredObject.ResourceName.Equals(resourceName)) &&
                    (deviation.MonitoredObject.ResourceType == resourceType))
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        // VH - Removed 6/2012 - Not used?
        //private bool IsThresholdEnabled(Metric metric, ScheduledRefreshState state, MonitoredObjectStateGraph stateGraph)
        //{
        //    return IsThresholdEnabled((int) metric, state, stateGraph);
        //}

        //private bool IsThresholdEnabled(int metricID, ScheduledRefreshState state, MonitoredObjectStateGraph stateGraph)
        //{
        //    MetricThresholdEntry thresholdEntry = null;
        //    state.Thresholds.TryGetValue(metricID, out thresholdEntry);
        //    if (thresholdEntry == null || !thresholdEntry.IsEnabled)
        //    {
        //        foreach (MetricState deviation in stateGraph.GetEvents())
        //        {
        //            if (deviation.MetricID == metricID)
        //                stateGraph.ClearEvent(deviation);
        //        }
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// Get the thresholds for this server and event
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        private MetricThresholdEntry GetThresholdEntry(Metric metric)
        {
            return GetDefaultThresholdEntry((int)metric);
        }

        private MetricThresholdEntry GetThresholdEntry(int metric)
        {
            return GetDefaultThresholdEntry(metric);
        }

        private MetricThresholdEntry GetThresholdEntry(Metric metric, string thresholdInstance)
        {
            return GetThresholdEntry((int)metric, thresholdInstance);
        }

        private MetricThresholdEntry GetThresholdEntry(int metric, string thresholdInstance)
        {
            MetricThresholdEntry result = null;

            if (String.IsNullOrEmpty(thresholdInstance))
            {
                result = GetDefaultThresholdEntry(metric);
            }
            else
            {
                List<MetricThresholdEntry> mteList;
                if (thresholdInstances.TryGetValue(metric, out mteList))
                {
                    foreach (var mte in mteList)
                    {
                        if (mte.MetricInstanceName != thresholdInstance)
                        {
                            continue;
                        }
                        result = mte;
                        break;
                    }
                }
                if (result == null)
                {
                    result = GetDefaultThresholdEntry(metric);
                }
            }

            return result;
        }

        // VH - Removed 6/2012 - Not used?
        //private IDictionary<string, MetricThresholdEntry> GetThresholdEntries(Metric metric)
        //{
        //    return GetThresholdEntries((int) metric);
        //}

        private IDictionary<string, MetricThresholdEntry> GetThresholdEntries(int metric)
        {
            Dictionary<string, MetricThresholdEntry> result = new Dictionary<string, MetricThresholdEntry>();

            List<MetricThresholdEntry> mteList;
            if (thresholdInstances.TryGetValue(metric, out mteList))
            {
                foreach (var mte in mteList)
                {
                    result.Add(mte.IsDefaultThreshold ? String.Empty : mte.MetricInstanceName, mte);
                }
            }

            return result;
        }


        private MetricThresholdEntry GetDefaultThresholdEntry(int metric)
        {
            MetricThresholdEntry result = null;
            List<MetricThresholdEntry> mteList;
            if (thresholdInstances.TryGetValue(metric, out mteList))
            {
                foreach (var mte in mteList)
                {
                    if (mte.IsDefaultThreshold)
                    {
                        result = mte;
                        break;
                    }
                }
            }
            return result;
        }



        #region Processors

        private void ProcessVMMetrics(ScheduledRefresh refresh)
        {
            if (refresh == null)
                return;

            // Process VM Metrics 
            try
            {
                if (refresh.MonitoredServer.IsVirtualized)
                {
                    if (refresh.Server.VMConfig != null)
                    {
                        ProcessVirtualizationMetrics(refresh);
                    }
                    else
                    {
                        ReRaiseVMAlerts();
                        // TODO: Should consider adding alert to indicate that VM data was expected by not received.
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error in Process VM Metrics.  Continuing alert processing.", e);
            }
        }

        //private void ProcessSQLsafeMetrics(ScheduledRefresh refresh)
        //{
        //    if (refresh == null)
        //        return;

        //    try
        //    {
        //        if (refresh.MonitoredServer.IsSQLsafeConnected)
        //        {
        //            LOG.InfoFormat("{0} SQLsafe Operations were collected on this refresh", refresh.SQLsafeOperations.Count);

        //            if (refresh.SQLsafeOperations.Count > 0)
        //            {
        //                LOG.InfoFormat("{0} SQLsafe Operations were collected on this refresh", refresh.SQLsafeOperations.Count);

        //                int lastBackupOperationId = refresh.MonitoredServer.SQLsafeConfig.LastBackupActionId, 
        //                    lastDefragOperationId = refresh.MonitoredServer.SQLsafeConfig.LastDefragActionId;

        //                foreach(var operation in refresh.SQLsafeOperations)
        //                {
        //                    if (operation is SQLsafeDefragOperation)
        //                    {
        //                        lastDefragOperationId = operation.OperationId;
        //                    } else if (operation is SQLsafeBackupRestoreOperation)
        //                    {
        //                        lastBackupOperationId = operation.OperationId;
        //                    }
        //                }

        //                LOG.DebugFormat("Last backup operation ID for {0} is {1}", refresh.MonitoredServer.InstanceName, lastBackupOperationId);
        //                LOG.DebugFormat("Last defrag operation ID for {0} is {1}", refresh.MonitoredServer.InstanceName, lastDefragOperationId);

        //                refresh.MonitoredServer.SQLsafeConfig.LastBackupActionId = lastBackupOperationId;
        //                refresh.MonitoredServer.SQLsafeConfig.LastDefragActionId = lastDefragOperationId;
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        LOG.Error("Error in Process SQLsafe Metrics.  Continuing alert processing. ", e);
        //    }
        //}

        private bool ServerIsAccessible(ScheduledRefresh refresh)
        {
            MetricThresholdEntry serverThreshold = GetThresholdEntry(Metric.SqlServiceStatus);
            object serviceState = valueAdapter.GetServerValue(Metric.SqlServiceStatus, out collectedValue);

            if (serviceState == null ||
                ((ServiceState)serviceState) == ServiceState.UnableToConnect ||
                ((ServiceState)serviceState) == ServiceState.UnableToMonitor ||
                ((ServiceState)serviceState) == ServiceState.Paused)
            {
                // Don't Clear SQL Service Status or VM Alerts or metrics maintained over time with Alert Duration option
                List<int> excludedMetrics = new List<int>
                                    {
                                        (int) Metric.SqlServiceStatus,
                                        (int) Metric.VmCPUReadyWaitTime,
                                        (int) Metric.VmCPUUtilization,
                                        (int) Metric.VmConfigChange,
                                        (int) Metric.VmESXCPUUtilization,
                                        (int) Metric.VmESXMemorySwapDetected,
                                        (int) Metric.VmESXMemoryUsage,
                                        (int) Metric.VmESXPowerState,
                                        (int) Metric.VmHostServerChange,
                                        (int) Metric.VmMemorySwapDelayDetected,
                                        (int) Metric.VmMemoryUtilization,
                                        (int) Metric.VmPowerState,
                                        (int) Metric.VmReclaimedMemory,
                                        (int) Metric.VmResourceLimits,
                                        //metrics maintained over time with Alert Duration option
                                        (int) Metric.JobCompletion,
                                        (int) Metric.LongJobs,
                                        (int) Metric.LongJobsMinutes,
                                        (int) Metric.BombedJobs,
                                        (int) Metric.MirroringSessionRoleChange,
                                        (int) Metric.ClusterFailover,
                                        (int) Metric.ReorganisationPct
                                    };

                // Clear remaining alerts because the server is not accessible
                ClearEvents(stateGraph, excludedMetrics);
                return false;
            }

            if (refresh.CollectionFailed || refresh.Server.CollectionFailed)
            {
                return false;
            }

            return true;
        }

        // Modification Start ID: M1
        /// <summary>
        /// Pruthviraj Nikam     Date: 16-Jan-2019
        /// The SQL Server Browser Services does not need to be checked if; 
        /// 1) There is only a default instance of SQL Server installed on the server, and 
        /// 2) The Remote Administrative Connnections feature is disabled. 
        /// The browser service is not required if those two conditions are true.
        /// </summary>
        /// <returns></returns>
        private bool CheckSQLBrowserServiceStatus(ScheduledRefresh refresh)
        {
            // PrescriptiveAnalyzer.Common.Helpers.SQLHelper.
            // 1) There is only a default instance of SQL Server installed on the server, and
            bool instancesResult = !refresh.Server.MultipleDefaultInstancesPresent;

            // 2) The Remote Administrative Connnections feature is disabled.          
            bool racResult = refresh.ConfigurationDetails.ConfigurationSettings.Select("Name = 'remote admin connections'").Any();

            return instancesResult && racResult;
        }
        // Modification End ID: M1


        private void ProcessSimpleAlerts(ScheduledRefresh refresh)
        {

            //M1:
            // If the SQL Server Agent is not running then SQL Server Agent XPs will be disabled anyway. 
            // It is not necessary to check SQL Server Agent XPs if the service is not running
            if (refresh.Server.AgentServiceStatus != ServiceState.UnableToMonitor &&
                 refresh.Server.AgentServiceStatus != ServiceState.Running)
                ProcessServerMetric(Metric.AgentServiceStatus);

            ProcessServerMetric(Metric.DtcServiceStatus);
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --process the new sql services
            // M1
            if (CheckSQLBrowserServiceStatus(refresh))
                ProcessServerMetric(Metric.SQLBrowserServiceStatus);

            ProcessServerMetric(Metric.SQLActiveDirectoryHelperServiceStatus);
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --process the new sql services

            if (refresh.ProductVersion.Major < 10)
            {
                if (isServerSqlExpress)  //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition 
                {
                    if (isFullTextInstalled && IsMetricValidForSqlExpress(Metric.FullTextServiceStatus))
                    {
                        ProcessServerMetric(Metric.FullTextServiceStatus, valueAdapter.GetServerValue);
                    }
                }
                else
                    ProcessServerMetric(Metric.FullTextServiceStatus, valueAdapter.GetServerValue);

            }

            ProcessServerMetric(Metric.SQLCPUUsagePct);
            ProcessServerMetric(Metric.UserConnectionPct);
            ProcessServerMetric(Metric.ReadWriteErrors);
            ProcessServerMetric(Metric.SQLMemoryUsagePct);
            ProcessServerMetric(Metric.ServerResponseTime);

            if (refresh.MonitoredServer.ReplicationMonitoringDisabled)
            {


                if (isServerSqlExpress)  //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition 
                {
                    if (IsMetricValidForSqlExpress(Metric.NonDistributedTrans))
                        CheckEvents((int)Metric.NonDistributedTrans, serverName, MonitoredState.OK, 0, false, null, null, false);

                    if (IsMetricValidForSqlExpress(Metric.NonDistributedTransTime))
                        CheckEvents((int)Metric.NonDistributedTransTime, serverName, MonitoredState.OK, 0, false, null, null, false);
                }
                else
                {
                    CheckEvents((int)Metric.NonDistributedTrans, serverName, MonitoredState.OK, 0, false, null, null, false);
                    CheckEvents((int)Metric.NonDistributedTransTime, serverName, MonitoredState.OK, 0, false, null, null, false);
                }

                CheckEvents((int)Metric.NonSubscribedTransNum, serverName, MonitoredState.OK, 0, false, null, null, false);
                CheckEvents((int)Metric.NonSubscribedTransTime, serverName, MonitoredState.OK, 0, false, null, null, false);
            }
            else
            {
                ProcessServerMetric(Metric.NonDistributedTrans);
                ProcessServerMetric(Metric.NonDistributedTransTime);
                ProcessServerMetric(Metric.NonSubscribedTransNum);
                ProcessServerMetric(Metric.NonSubscribedTransTime);
            }

            ProcessServerMetric(Metric.CLRStatus);
            ProcessServerMetric(Metric.OLEAutomationStatus);
            ProcessServerMetric(Metric.QueryMonitorStatus);
            // M1
            // If the SQL Server Agent is not running then SQL Server Agent XPs will be disabled anyway. 
            // It is not necessary to check SQL Server Agent XPs if the service is not running
            //if (refresh.Server.AgentServiceStatus != ServiceState.UnableToMonitor &&
            //    refresh.Server.AgentServiceStatus != ServiceState.Running)
            //    ProcessServerMetric(Metric.AgentXPStatus);
            // Fix for SQLDM-30283 - Process AgentXPStatus metric only if AgentServiceStatus is Running
            if (refresh.Server.AgentServiceStatus == ServiceState.Running)
                ProcessServerMetric(Metric.AgentXPStatus);

            //
            ProcessServerMetric(Metric.WMIStatus);
            ProcessServerMetric(Metric.ClientComputers);


            ProcessServerMetric(Metric.BlockedSessions);

            ProcessServerMetric(Metric.OSMetricsStatus);
            //10.0 SQLdm srishti purohit -- to supply baseline value to GetServerOSValues function
            MetricThresholdEntry serverThreshold = GetThresholdEntry(Metric.OSMetricsStatus);
            IComparable osMetricStatus = valueAdapter.GetServerOSValue(Metric.OSMetricsStatus, out collectedValue);
            if (osMetricStatus.CompareTo(OSMetricsStatus.Available) == 0)
            {
                ProcessServerMetric(Metric.OSCPUPrivilegedTimePct);
                ProcessServerMetric(Metric.OSCPUProcessorQueueLength);
                ProcessServerMetric(Metric.OSCPUUsagePct);
                ProcessServerMetric(Metric.OSDiskPhysicalDiskTimePct);
                ProcessServerMetric(Metric.OSDiskAverageDiskQueueLength);
                ProcessServerMetric(Metric.OSMemoryUsagePct);
                ProcessServerMetric(Metric.OSMemoryPagesPerSecond);
                ProcessServerMetric(Metric.OSUserCPUUsagePct);
            }

            ProcessServerMetric(Metric.PageLifeExpectancy);
            ProcessServerMetric(Metric.ProcCacheHitRatio);
            ProcessServerTempdbMetric(Metric.VersionStoreGenerationRatio);
            if (refresh.ProductVersion.Major > 8)
            {
                ProcessServerTempdbMetric(Metric.VersionStoreSize);
            }
            ProcessServerTempdbMetric(Metric.TempdbContention);


            ProcessMirroringMetrics(refresh);
        }

        private void ProcessComplexAlerts(ScheduledRefresh refresh)
        {
            ProcessTableFragmentation(refresh);
            ProcessSessionAlerts(refresh);
            ProcessJobAlerts(refresh);
            ProcessCustomCounterAlerts(refresh);

            ProcessLogAlerts(refresh);
            ProcessClusterAlerts(refresh);
            ProcessAlwaysOnMetrics(refresh);
        }

        private void ClearNonContinuingAlerts()
        {
            // if threshold is no longer enabled for following events - clear existing state
            if (!IsThresholdEnabled(Metric.OldestOpenTransMinutes))
            {
                ClearEvents(Metric.OldestOpenTransMinutes);
            }
            if (!IsThresholdEnabled(Metric.BlockingAlert))
            {
                ClearEvents(Metric.BlockingAlert);
            }
            if (!IsThresholdEnabled(Metric.ResourceAlert))
            {
                ClearEvents(Metric.ResourceAlert);
            }
            if (!IsThresholdEnabled(Metric.BombedJobs))
            {
                ClearEvents(Metric.BombedJobs);
            }
            if (!IsThresholdEnabled(Metric.LongJobs))
            {
                ClearEvents(Metric.LongJobs);
            }
            if (!IsThresholdEnabled(Metric.LongJobsMinutes))
            {
                ClearEvents(Metric.LongJobsMinutes);
            }
            if (!IsThresholdEnabled(Metric.JobCompletion))
            {
                ClearEvents(Metric.JobCompletion);
            }
        }

        #endregion

        #region complex processors

        #region Session Alerts

        private void ProcessSessionAlerts(ScheduledRefresh refresh)
        {
            ProcessOldestOpenTransaction(refresh);
            ProcessOldestOpenVersionStoreTransaction(refresh);
            ProcessTempdbSpaceUsage(refresh);
            ProcessBlockingSessions(refresh);
            ProcessResourceAlert(refresh);
            ProcessDeadlocks(refresh);
        }

        private void ProcessOldestOpenTransaction(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;
            object metricValue = null;

            // process oldest open transactions
            MetricThresholdEntry openTransactionsThreshold = GetThresholdEntry(Metric.OldestOpenTransMinutes);
            metricDeviationMap = stateGraph.GetEvents(Metric.OldestOpenTransMinutes);
            if ((openTransactionsThreshold != null) && openTransactionsThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(openTransactionsThreshold);

                foreach (OpenTransaction xa in refresh.Alerts.OldestOpenTransactions)
                {
                    if (xa.RunTime == null)
                    {
                        continue;
                    }

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, xa.Database);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              xa.Spid.ToString(),
                                                              xa.StartTime.ToString()
                                                          };
                    if (openTransactionsThreshold.IsBaselineEnabled)
                    {
                        LOG.Verbose("Getting baseline value to compare for alert for metric {0}.", (int)Metric.OldestOpenTransMinutes);
                        double? meanValue;
                        if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.OldestOpenTransMinutes, out meanValue))
                        {
                            xa.TransMinutesBaselineMean = meanValue;
                            if (xa.TransMinutesBaselineMean != null || xa.TransMinutesBaselineMean != 0)
                                xa.OpenTransMinutesPercDeviationBaseine = ((int)xa.RunTime.Value.TotalMinutes / xa.TransMinutesBaselineMean) * 100;
                            else
                                xa.OpenTransMinutesPercDeviationBaseine = null;
                            value = xa.OpenTransMinutesPercDeviationBaseine;
                        }
                    }
                    else
                        value = (int)xa.RunTime.Value.TotalMinutes;
                    metricValue = (int)xa.RunTime.Value.TotalMinutes;

                    state = ScheduledRefreshState.GetState(value, openTransactionsThreshold);
                    if (state <= MonitoredState.OK)
                    {
                        continue;
                    }

                    CheckEvents((int)Metric.OldestOpenTransMinutes, objectName, state, value, false, xa, advancedSettings, false, openTransactionsThreshold.IsBaselineEnabled, metricValue);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }

            // raise clear events for any remaining alerts
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.OldestOpenTransMinutes, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
            }
        }

        private void ProcessOldestOpenVersionStoreTransaction(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process oldest open version store transaction
            MetricThresholdEntry openVSTransactionsThreshold = GetThresholdEntry(Metric.LongRunningVersionStoreTransaction);
            metricDeviationMap = stateGraph.GetEvents(Metric.LongRunningVersionStoreTransaction);
            if ((openVSTransactionsThreshold != null)
                && openVSTransactionsThreshold.IsEnabled
                && refresh.SessionList != null
                && refresh.SessionList.SessionList != null &&
                refresh.SessionList.SessionList.Count > 0)
            {
                advancedSettings = GetAdvancedConfiguration(openVSTransactionsThreshold);

                Pair<int?, DateTime?> longestTransactionIdentifer = new Pair<int?, DateTime?>();
                TimeSpan? longestTransactionLength = null;

                foreach (Session s in refresh.SessionList.SessionList.Values)
                {
                    if (s.VersionStoreElapsedTime.HasValue)
                    {
                        if (!longestTransactionLength.HasValue)
                        {
                            longestTransactionLength = s.VersionStoreElapsedTime;
                            longestTransactionIdentifer = s.InternalSessionIdentifier;
                        }
                        else
                        {
                            if (s.VersionStoreElapsedTime.Value.TotalSeconds < longestTransactionLength.Value.TotalSeconds)
                            {
                                longestTransactionLength = s.VersionStoreElapsedTime;
                            }
                            longestTransactionIdentifer = s.InternalSessionIdentifier;
                        }
                    }
                }

                if (longestTransactionLength.HasValue && refresh.SessionList.SessionList.ContainsKey(longestTransactionIdentifer)
                    && refresh.SessionList.SessionList[longestTransactionIdentifer].VersionStoreElapsedTime.HasValue)
                {
                    Session longestTransaction = refresh.SessionList.SessionList[longestTransactionIdentifer];

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, "tempdb");
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              longestTransaction.Spid.ToString(),
                                                              longestTransaction.Workstation,
                                                              longestTransaction.InternalSessionIdentifier.First.ToString(),
                                                              longestTransaction.InternalSessionIdentifier.Second.ToString()
                                                          };

                    value = (int)longestTransaction.VersionStoreElapsedTime.Value.TotalMinutes;

                    state = ScheduledRefreshState.GetState(value, openVSTransactionsThreshold);

                    Pair<Session, object[]> additionalData = new Pair<Session, object[]>(longestTransaction,
                                                                                         ScheduledRefreshValueAdapter.
                                                                                             GetVersionStoreAdditionalValue
                                                                                             (refresh));

                    CheckEvents((int)Metric.LongRunningVersionStoreTransaction, objectName, state, value, false, additionalData,
                                advancedSettings, false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }

            // raise clear events for any remaining alerts
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.LongRunningVersionStoreTransaction, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
            }
        }

        private void ProcessTempdbSpaceUsage(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry sessionTempdbSpaceUsageThreshold = GetThresholdEntry(Metric.SessionTempdbSpaceUsage);
            metricDeviationMap = stateGraph.GetEvents(Metric.SessionTempdbSpaceUsage);
            if (sessionTempdbSpaceUsageThreshold != null)
            {
                advancedSettings = GetAdvancedConfiguration(sessionTempdbSpaceUsageThreshold);
            }
            else
            {
                advancedSettings = null;
            }
            if (sessionTempdbSpaceUsageThreshold.IsEnabled)
            {
                if (refresh.SessionList != null)
                {
                    foreach (Session s in refresh.SessionList.SessionList.Values)
                    {
                        if (s.TotalTempdbSpaceUsed != null && s.TotalTempdbSpaceUsed.Megabytes.HasValue)
                        {
                            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, "tempdb");
                            objectName.AdditionalQualifiers = new string[]
                                                                  {
                                                                      s.Spid.ToString(),
                                                                      s.Workstation,
                                                                      s.InternalSessionIdentifier.First.ToString(),
                                                                      s.InternalSessionIdentifier.Second.ToString()
                                                                  };

                            value = Convert.ToInt32(s.TotalTempdbSpaceUsed.Megabytes);

                            state = ScheduledRefreshState.GetState(value, sessionTempdbSpaceUsageThreshold);

                            CheckEvents((int)Metric.SessionTempdbSpaceUsage, objectName, state, value, false, s, advancedSettings, false);

                            if (metricDeviationMap.ContainsKey(objectName))
                            {
                                metricDeviationMap.Remove(objectName);
                            }
                        }
                    }
                }
            }

            // raise clear events for any remaining alerts
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.SessionTempdbSpaceUsage, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, advancedSettings,
                            false);
            }
        }


        /// <summary>
        /// Look for blocking detected by activity monitor (between refreshes)
        /// </summary>
        /// <param name="refresh"></param>
        private void AugmentBlockingFromActivityMonitor(ScheduledRefresh refresh)
        {
            //iterate through data from trace
            foreach (var info in refresh.BlockingSessions.Values)
            {
                bool found = false;
                //if there is a match in Blocking Sessions
                foreach (var session in refresh.Alerts.BlockingSessions)
                {
                    if (session.Spid.HasValue)
                    {
                        if (info.LastBatchCompleted.HasValue && session.BlockingLastBatch.HasValue)
                        {
                            if (info.SessionId == session.Spid)
                            {
                                found = true;
                                //give the sessions from sysprocesses an XActID
                                session.xActID = info.XActId;
                                session.BlockID = info.GetId();
                                //trace will have a more accurate blocking time
                                session.BlockingTime = info.BlockingTime;
                                session.WaitResource = info.WaitResource;
                                session.IsInterRefresh = true;
                            }
                        }
                    }
                }
                if (!found)
                {
                    var newSession = new BlockingSession
                    {
                        BlockingStartTimeUTC = info.BlockingStartTimeUtc, //.ToUniversalTime(),
                        Spid = info.SessionId,
                        Databasename = info.DatabaseName,
                        Application = info.ClientApp,
                        Host = info.Host,
                        Login = info.Login,
                        InputBuffer = info.InputBuffer,
                        ObjectId = info.ObjectID,
                        BlockingTime = info.BlockingTime,
                        BlockingLastBatch = info.LastBatchCompleted,
                        xActID = info.XActId,
                        BlockID = info.GetId(),
                        WaitResource = info.WaitResource,
                        IsInterRefresh = true
                    };

                    refresh.Alerts.BlockingSessions.Add(newSession);
                }
            }
        }

        /// <summary>
        /// Alert on blocking sessions
        /// </summary>
        /// <param name="refresh"></param>
        private void ProcessBlockingSessions(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;

            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process blocking alerts
            MetricThresholdEntry blockingThreshold = GetThresholdEntry(Metric.BlockingAlert);
            metricDeviationMap = stateGraph.GetEvents(Metric.BlockingAlert);

            //if the blocking alert is enabled
            if ((blockingThreshold != null) && blockingThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(blockingThreshold);

                AugmentBlockingFromActivityMonitor(refresh);

                //refresh.Alerts.BlockingSessions will contain blocking sessions from the trace file (if enabled) and sysprocesses
                foreach (BlockingSession session in refresh.Alerts.BlockingSessions)
                {

                    // get the state of this session
                    value = (int)session.BlockingTime.TotalSeconds;

                    state = ScheduledRefreshState.GetState(value, blockingThreshold);
                    if (state <= MonitoredState.OK)
                    {
                        continue;
                    }

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, session.Databasename);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              session.Spid.ToString(),
                                                              session.Host,
                                                              session.WaitResource,
                                                              session.xActID.ToString()
                                                          };


                    //This will add it to refresh.Events
                    //What it adds to events it should remove from metricdeviationmap to ensure it does not get lowered to ok
                    //So it seems that this is a different object name
                    CheckEvents((int)Metric.BlockingAlert, objectName, state, value, false, session, advancedSettings, false);

                    //remove it from metricdeviationmap if we want the alert to remain
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }

            //OK all alerts that are in metricdeviationmap
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.BlockingAlert, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
            }
        }

        private void ProcessResourceAlert(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process resource limiter alerts
            MetricThresholdEntry resourceThreshold = GetThresholdEntry(Metric.ResourceAlert);
            metricDeviationMap = stateGraph.GetEvents(Metric.ResourceAlert);
            if ((resourceThreshold != null) && resourceThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(resourceThreshold);

                foreach (Session session in refresh.Alerts.ResourceCheckSessions)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, session.Database);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              session.Spid.ToString(),
                                                              session.LoggedInSince.ToString(),
                                                              session.LastCommand
                                                          };
                    value = session.Cpu.TotalSeconds;
                    if (resourceThreshold.CriticalThreshold.Enabled && resourceThreshold.CriticalThreshold.IsInViolation(value))
                    {
                        state = MonitoredState.Critical;
                    }
                    else if (resourceThreshold.WarningThreshold.Enabled && resourceThreshold.WarningThreshold.IsInViolation(value))
                    {
                        state = MonitoredState.Warning;
                    }
                    else if (resourceThreshold.InfoThreshold.Enabled && resourceThreshold.InfoThreshold.IsInViolation(value))
                    {
                        state = MonitoredState.Informational;
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    CheckEvents((int)Metric.ResourceAlert, objectName, state, value, false, session, advancedSettings, false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.ResourceAlert, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, advancedSettings, false);
            }
        }

        private void ProcessDeadlocks(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry deadlockThreshold = GetThresholdEntry(Metric.Deadlock);
            metricDeviationMap = stateGraph.GetEvents(Metric.Deadlock);
            if ((deadlockThreshold != null) && deadlockThreshold.IsEnabled
                && refresh.Deadlocks != null && refresh.Deadlocks.Count > 0)
            {
                advancedSettings = GetAdvancedConfiguration(deadlockThreshold);

                foreach (DeadlockInfo d in refresh.Deadlocks)
                {
                    if (deadlockThreshold.CriticalThreshold.Enabled)
                    {
                        state = MonitoredState.Critical;
                    }
                    else if (deadlockThreshold.WarningThreshold.Enabled)
                    {
                        state = MonitoredState.Warning;
                    }
                    else if (deadlockThreshold.InfoThreshold.Enabled)
                    {
                        state = MonitoredState.Informational;
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    value = state;

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[] { d.GetId().ToString() };

                    CheckEvents((int)Metric.Deadlock, objectName, state, value, false, d, advancedSettings, false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            // clear state for deadlocks generated on prior refresh
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                stateGraph.ClearEvent(deviation);
            }
        }

        #endregion

        #region Job Alerts

        private void ProcessJobAlerts(ScheduledRefresh refresh)
        {
            if (isServerSqlExpress)  //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition 
            {
                if (IsMetricValidForSqlExpress(Metric.BombedJobs))
                    ProcessFailedJobs(refresh);

                if (IsMetricValidForSqlExpress(Metric.JobCompletion))
                    ProcessCompletedJobs(refresh);

                if (IsMetricValidForSqlExpress(Metric.LongJobs))
                    ProcessLongRunningJobsPercent(refresh);

                if (IsMetricValidForSqlExpress(Metric.LongJobsMinutes))
                    ProcessLongRunningJobsMinutes(refresh);
            }
            else
            {
                ProcessFailedJobs(refresh);
                ProcessCompletedJobs(refresh);
                ProcessLongRunningJobsPercent(refresh);
                ProcessLongRunningJobsMinutes(refresh);
            }

        }

        private void ProcessFailedJobs(ScheduledRefresh refresh)  // Also called bombed jobs
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            /* process bombed jobs
             * Bombed jobs are tracked by server and job name.  These alerts are never cleared, that is
             * they will never have an OK alert generated for them.  They will become inactive alerts when
             * either the same job fails again or after 12 hours.
             */
            advancedSettings = null;
            MetricThresholdEntry bombedJobsThreshold = GetThresholdEntry(Metric.BombedJobs);
            metricDeviationMap = stateGraph.GetEvents(Metric.BombedJobs);
            if ((bombedJobsThreshold != null) && bombedJobsThreshold.IsEnabled && isServerAccessible)
            {
                Set<string> jobList = new Set<string>();
                advancedSettings = GetAdvancedConfiguration(bombedJobsThreshold);

                foreach (AgentJobFailure job in refresh.Alerts.JobFailures.JobList)
                {
                    //SQLDM-28607
                    if (job.Command.Contains("Password"))
                    {
                        string newCommand = replacePasswordWithAsterisk(job);
                        if(newCommand != null)
                        {
                            job.Command = newCommand;
                        }
                    }
                    if (bombedJobsThreshold.CriticalThreshold.Enabled)
                    {
                        state = MonitoredState.Critical;
                    }
                    else if (bombedJobsThreshold.WarningThreshold.Enabled)
                    {
                        state = MonitoredState.Warning;
                    }
                    else if (bombedJobsThreshold.InfoThreshold.Enabled)
                    {
                        state = MonitoredState.Informational;
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    value = state;

                    // Raise alert for the job step if job step alerting is turned on and the stepId triggering the failure is not step 0
                    if (advancedSettings.AlertOnJobSteps && job.StepId != 0)
                    {
                        MonitoredObjectName objectNameForStep = new MonitoredObjectName(serverName.ServerName);
                        objectNameForStep.AdditionalQualifiers = new string[]
                                                                     {
                                                                         job.JobName,
                                                                         job.StepName
                                                                     };

                        CheckEvents((int)Metric.BombedJobs, objectNameForStep, state, value, false, job, advancedSettings, false);

                        if (metricDeviationMap.ContainsKey(objectNameForStep))
                        {
                            metricDeviationMap.Remove(objectNameForStep);
                        }
                    }
                    else
                    {
                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                        objectName.AdditionalQualifiers = new string[]
                                                              {
                                                                  job.JobName
                                                              };

                        CheckEvents((int)Metric.BombedJobs, objectName, state, value, false, job, advancedSettings, false);

                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                }

                foreach (AgentJobClear job in refresh.Alerts.JobFailures.ClearJobList)
                {
                   
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              job.JobName
                                                          };

                    state = MonitoredState.OK;
                    value = state;

                    CheckEvents((int)Metric.BombedJobs, objectName, state, value, false, job, advancedSettings, false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }

            // include previously generated deviations that have not cleared and are less than 12 hours old
            DateTime inactiveTime = DateTime.UtcNow - GetJobAlertsActiveDuration(advancedSettings);
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                if (deviation.LastUpdate >= inactiveTime)
                {
                    if (!deviation.Acknowledged && bombedJobsThreshold.IsEnabled)
                    {
                        if (bombedJobsThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (bombedJobsThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else if (bombedJobsThreshold.InfoThreshold.Enabled)
                        {
                            state = MonitoredState.Informational;
                        }
                        else
                        {
                            state = MonitoredState.OK;
                        }

                        CheckEvents((int)Metric.BombedJobs, deviation.MonitoredObject, state, (IComparable)deviation.Value, false, deviation.AdditionalData,
                                    advancedSettings, true);
                    }
                }
                else
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        //SQLDM-28607
        private string replacePasswordWithAsterisk(AgentJobFailure job)
        {
            string command = job.Command;
            string newCommand = null;
            string password = null;
            int indexOfPwd = command.IndexOf("Password");
            int endIndex = indexOfPwd + command.Substring(indexOfPwd).IndexOf("Par");
            if(endIndex > 0)
            {
                password = command.Substring(indexOfPwd + 11, endIndex - (indexOfPwd + 11));
                newCommand = command.Replace(password, "******");
            }
            return newCommand;
         }

        private void ProcessCompletedJobs(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process completed jobs
            advancedSettings = null;
            MetricThresholdEntry completedJobsThreshold = GetThresholdEntry(Metric.JobCompletion);
            metricDeviationMap = stateGraph.GetEvents(Metric.JobCompletion);
            if ((completedJobsThreshold != null) && completedJobsThreshold.IsEnabled && isServerAccessible)
            {
                advancedSettings = GetAdvancedConfiguration(completedJobsThreshold);

                foreach (AgentJobCompletion job in refresh.Alerts.JobsCompleted.JobList)
                {
                    JobStepCompletionStatus? runStatus = job.RunStatus;
                    value = runStatus.HasValue ? runStatus.Value : (IComparable)JobStepCompletionStatus.Unknown;

                    state = ScheduledRefreshState.GetState(value, completedJobsThreshold);

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[] { job.JobName, job.StepName };

                    CheckEvents((int)Metric.JobCompletion, objectName, state, value, false, job, advancedSettings, false);
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }

            // include previously generated deviations that have not cleared and are less than 12 hours old
            DateTime inactiveTime = DateTime.UtcNow - GetJobAlertsActiveDuration(advancedSettings);
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                if (deviation.LastUpdate >= inactiveTime)
                {
                    if (!deviation.Acknowledged && completedJobsThreshold.IsEnabled)
                    {
                        value = (IComparable)deviation.Value;

                        state = ScheduledRefreshState.GetState(value, completedJobsThreshold);
                        if (state <= MonitoredState.OK)
                        {
                            stateGraph.ClearEvent(deviation);
                            continue;
                        }

                        CheckEvents((int)Metric.JobCompletion, deviation.MonitoredObject, state, value, false, deviation.AdditionalData, advancedSettings, true);
                    }
                }
                else
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        private void ProcessLongRunningJobsPercent(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process long running jobs
            advancedSettings = null;
            MetricThresholdEntry longJobsThreshold = GetThresholdEntry(Metric.LongJobs);
            metricDeviationMap = stateGraph.GetEvents(Metric.LongJobs);
            if ((longJobsThreshold != null) && longJobsThreshold.IsEnabled && isServerAccessible)
            {
                advancedSettings = GetAdvancedConfiguration(longJobsThreshold);

                foreach (AgentJobRunning job in refresh.Alerts.LongRunningJobs)
                {
                    if (job.AssociatedThreshold != Metric.LongJobs)
                    {
                        continue;
                    }

                    //Job level
                    value = (int)job.RunTimePercentOver;
                    state = ScheduledRefreshState.GetState(value, longJobsThreshold);

                    if (state >= MonitoredState.OK)
                    {
                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                        objectName.AdditionalQualifiers = new string[]
                                                              {
                                                                  job.JobName,
                                                                  "jobLevel"
                                                              };

                        CheckEvents((int)Metric.LongJobs, objectName, state, value, false, job, advancedSettings, false);
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }

                    //Step level
                    if (advancedSettings.AlertOnJobSteps)
                    {
                        value = (int)job.StepRunTimePercentOver;
                        state = ScheduledRefreshState.GetState(value, longJobsThreshold);

                        if (state >= MonitoredState.OK)
                        {
                            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                            objectName.AdditionalQualifiers = new string[]
                                                                  {
                                                                      job.JobName,
                                                                      "stepLevel",
                                                                      job.StepName
                                                                  };

                            CheckEvents((int)Metric.LongJobs, objectName, state, value, false, job, advancedSettings, false);
                            if (metricDeviationMap.ContainsKey(objectName))
                            {
                                metricDeviationMap.Remove(objectName);
                            }
                        }
                    }
                }
            }

            // include previously generated deviations that have not cleared and are less than 12 hours old
            DateTime inactiveTime = DateTime.UtcNow - GetJobAlertsActiveDuration(advancedSettings);
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                if (deviation.LastUpdate >= inactiveTime)
                {
                    if (!deviation.Acknowledged && longJobsThreshold.IsEnabled)
                    {
                        value = (int)deviation.Value;
                        // OK is now a valid state because the item could have been added for the LongJobsMinutes threshold was exceeded
                        state = ScheduledRefreshState.GetState(value, longJobsThreshold);
                        if (state <= MonitoredState.OK)
                        {
                            // if the threshold changed and its no longer a problem then stop raising it
                            stateGraph.ClearEvent(deviation);
                            continue;
                        }

                        CheckEvents((int)Metric.LongJobs, deviation.MonitoredObject, state, value, false, deviation.AdditionalData, advancedSettings, true);
                    }
                }
                else
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        private void ProcessLongRunningJobsMinutes(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process long running jobs
            MetricThresholdEntry longJobsThreshold = GetThresholdEntry(Metric.LongJobsMinutes);
            advancedSettings = null;
            metricDeviationMap = stateGraph.GetEvents(Metric.LongJobsMinutes);
            if ((longJobsThreshold != null) && longJobsThreshold.IsEnabled && isServerAccessible)
            {
                advancedSettings = GetAdvancedConfiguration(longJobsThreshold);

                foreach (AgentJobRunning job in refresh.Alerts.LongRunningJobs)
                {
                    if (job.AssociatedThreshold != Metric.LongJobsMinutes)
                    {
                        continue;
                    }

                    //Job level
                    value = (int)job.RunningTime.TotalMinutes;
                    state = ScheduledRefreshState.GetState(value, longJobsThreshold);

                    if (state >= MonitoredState.OK)
                    {
                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                        objectName.AdditionalQualifiers = new string[]
                                                              {
                                                                  job.JobName,
                                                                  "jobLevel"
                                                              };

                        CheckEvents((int)Metric.LongJobsMinutes, objectName, state, value, false, job, advancedSettings,
                                    false);
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }

                    //Step level
                    if (advancedSettings.AlertOnJobSteps)
                    {
                        value = (int)job.StepRunningTime.TotalMinutes;
                        state = ScheduledRefreshState.GetState(value, longJobsThreshold);

                        if (state >= MonitoredState.OK)
                        {
                            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                            objectName.AdditionalQualifiers = new string[]
                                                                  {
                                                                      job.JobName,
                                                                      "stepLevel",
                                                                      job.StepName
                                                                  };

                            CheckEvents((int)Metric.LongJobsMinutes, objectName, state, value, false, job, advancedSettings,
                                        false);
                            if (metricDeviationMap.ContainsKey(objectName))
                            {
                                metricDeviationMap.Remove(objectName);
                            }
                        }
                    }
                }
            }

            // include previously generated deviations that have not cleared and are less than 12 hours old
            DateTime inactiveTime = DateTime.UtcNow - GetJobAlertsActiveDuration(advancedSettings);
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                if (deviation.LastUpdate >= inactiveTime)
                {
                    if (!deviation.Acknowledged && longJobsThreshold.IsEnabled)
                    {
                        value = (int)deviation.Value;
                        // OK is now a valid state because the item could have been added for the LongJobsMinutes threshold was exceeded
                        state = ScheduledRefreshState.GetState(value, longJobsThreshold);
                        if (state <= MonitoredState.OK)
                        {
                            // if the threshold changed and its no longer a problem then stop raising it
                            stateGraph.ClearEvent(deviation);
                            continue;
                        }

                        CheckEvents((int)Metric.LongJobsMinutes, deviation.MonitoredObject, state, value, false, deviation.AdditionalData, advancedSettings, true);
                    }
                }
                else
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        private void ProcessCustomCounterAlerts(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            Set<int> customCounterKeys = Collection.GetCustomCounterKeySet();
            MetricThresholdEntry customCounterThreshold = null;
            foreach (CustomCounterSnapshot ccs in refresh.CustomCounters.Values)
            {
                state = MonitoredState.None;

                CustomCounterDefinition ccd = ccs.Definition;
                int metricID = ccd.MetricID;

                // the shared counter definition gets updated when a counter is disabled
                CustomCounterDefinition globalCounterDef = Collection.GetCustomCounter(metricID);
                // no need to generate alerts for disabled counters
                if (globalCounterDef == null || !globalCounterDef.IsEnabled)
                {
                    continue;
                }

                customCounterKeys.Remove(metricID);
                customCounterThreshold = GetThresholdEntry(metricID);
                if ((customCounterThreshold != null) && customCounterThreshold.IsEnabled)
                {
                    advancedSettings = GetAdvancedConfiguration(customCounterThreshold);

                    if (ccs.CollectionFailed)
                    {
                        if (ccd.MetricType != MetricType.AzureCounter)
                        {

                            if (advancedSettings == null || !advancedSettings.TreatCustomCounterFailureAsCritical)
                            {
                                continue;
                            }
                           
                        }
                        state = MonitoredState.Critical;
                        value = 0m;
                    }
                    else
                    {
                        if (ccs.DisplayValue.HasValue)
                        {
                            try
                            {
                                value = ccs.DisplayValue.Value * Convert.ToDecimal(ccd.Scale);
                            }
                            catch (OverflowException oe)
                            {
                                ccs.SetError(
                                    "Custom counter value multiplied by the scaling factor exceeds the maximum value allowed for a custom counter.",
                                    oe);
                                value = ccs.DisplayValue.Value;
                                state = MonitoredState.Critical;
                            }
                        }
                        else
                        {
                            value = 0m;
                        }

                        if (state == MonitoredState.None)
                        {
                            state = ScheduledRefreshState.GetState(value, customCounterThreshold);
                        }
                    }

                    CheckEvents(metricID, serverName, state, value, false, ccs, advancedSettings, false);
                }
            }

            // build list of disabled custom counters 
            foreach (int metricID in customCounterKeys.ToArray())
            {
                CustomCounterDefinition ccd = Collection.GetCustomCounter(metricID);
                if (ccd.IsEnabled)
                {
                    customCounterKeys.Remove(metricID);
                }
            }

            // clear any outstanding events for counters that are now disabled
            // ---- events are cleared but not added to the scheduled refresh 
            // ---- the MS will clean up its own state
            if (customCounterKeys.Count > 0)
            {
                stateGraph.ClearAllEvents(customCounterKeys.ToArray());
            }
        }

        #endregion

        #region disk alerts

        private void ProcessDiskAlerts(ScheduledRefresh refresh)
        {
            ProcessDiskFull(refresh);
            ProcessDiskTime(refresh);
            ProcessDiskQueueLength(refresh);
            ProcessDiskFree(refresh);
            ProcessAverageDiskMillisecondsPerRead(refresh);
            ProcessAverageDiskMillisecondsPerTransfer(refresh);
            ProcessAverageDiskMillisecondsPerWrite(refresh);
            ProcessDiskReadsPerSecond(refresh);
            ProcessDiskTransfersPerSec(refresh);
            ProcessDiskWritesPerSecond(refresh);
        }

        private void ProcessDiskFull(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            // process disk alerts 
            MetricThresholdEntry diskFullThreshold = null;
            metricDeviationMap = stateGraph.GetEvents(Metric.OSDiskFull);
            if (IsMetricEnabled(Metric.OSDiskFull)) // Multi-Instance Thresholds
            {
                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    diskFullThreshold = GetThresholdEntry(Metric.OSDiskFull, drive.DriveLetter);
                    if ((diskFullThreshold != null) && diskFullThreshold.IsThresholdEnabled)
                    {
                        advancedSettings = GetAdvancedConfiguration(diskFullThreshold);
                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, drive.DriveLetter, InstanceType.Disk);
                        //objectName.AdditionalQualifiers = new string[]
                        //                                      {
                        //                                          drive.DriveLetter
                        //                                      };
                        if (drive.PercentUsed != null)
                        {
                            value = drive.PercentUsed * 100;

                            state = ScheduledRefreshState.GetState(value, diskFullThreshold);

                            if (state <= MonitoredState.OK)
                            {
                                continue;
                            }

                            // The 'drive' object is important because it let to apply the disk's filters.
                            CheckEvents((int)Metric.OSDiskFull, objectName, state, value, false, drive,
                                        advancedSettings, false);
                        }
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.ResourceName))
                {
                    clearValue = refresh.DiskDrives[deviation.MonitoredObject.ResourceName].PercentUsed * 100;
                }

                CheckEvents((int)Metric.OSDiskFull, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, advancedSettings, false);
            }
        }

        private void ProcessDiskTime(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry diskTimeThreshold = GetThresholdEntry(Metric.OSDiskPhysicalDiskTimePctPerDisk);
            metricDeviationMap = stateGraph.GetEvents(Metric.OSDiskPhysicalDiskTimePctPerDisk);
            if ((diskTimeThreshold != null) && diskTimeThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(diskTimeThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };
                    // can't do squat without a disk busy percent
                    if (drive.DiskBusyPercent != null)
                    {
                        value = drive.DiskBusyPercent;

                        state = ScheduledRefreshState.GetState(value, diskTimeThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }
                        CheckEvents((int)Metric.OSDiskPhysicalDiskTimePctPerDisk, objectName, state, value, false, drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    clearValue = refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskBusyPercent;
                }

                CheckEvents((int)Metric.OSDiskPhysicalDiskTimePctPerDisk, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null,
                            false);
            }
        }

        private void ProcessDiskQueueLength(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry diskQueueThreshold = GetThresholdEntry(Metric.OSDiskAverageDiskQueueLengthPerDisk);
            metricDeviationMap = stateGraph.GetEvents(Metric.OSDiskAverageDiskQueueLengthPerDisk);
            if ((diskQueueThreshold != null) && diskQueueThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(diskQueueThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    // can't do squat without a avg disk queue length
                    if (drive.AverageDiskQueueLength != null)
                    {
                        value = drive.AverageDiskQueueLength;

                        state = ScheduledRefreshState.GetState(value, diskQueueThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }
                        CheckEvents((int)Metric.OSDiskAverageDiskQueueLengthPerDisk, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    clearValue = refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].AverageDiskQueueLength;
                }

                CheckEvents((int)Metric.OSDiskAverageDiskQueueLengthPerDisk, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData,
                            null, false);
            }
        }

        private void ProcessDiskFree(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            //  TODO: NEED TO CONSOLIDATE THE DISK METRICS INTO A SINGLE METHOD THAT ONLY HAS TO LOOP THROUGH THE DISKS ONCE
            MetricThresholdEntry diskFreeMbThreshold = null;
            metricDeviationMap = stateGraph.GetEvents(Metric.OsDiskFreeMb);
            if (IsMetricEnabled(Metric.OsDiskFreeMb))
            {
                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    diskFreeMbThreshold = GetThresholdEntry(Metric.OsDiskFreeMb, drive.DriveLetter);
                    if ((diskFreeMbThreshold != null) && diskFreeMbThreshold.IsThresholdEnabled)
                    {
                        advancedSettings = GetAdvancedConfiguration(diskFreeMbThreshold);

                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, drive.DriveLetter, InstanceType.Disk);
                        //objectName.AdditionalQualifiers = new string[]
                        //                                      {
                        //                                          drive.DriveLetter
                        //                                      };
                        if (drive.PercentUsed != null)
                        {
                            value = drive.UnusedSize.Megabytes;

                            state = ScheduledRefreshState.GetState(value, diskFreeMbThreshold);

                            if (state <= MonitoredState.OK)
                            {
                                continue;
                            }

                            CheckEvents((int)Metric.OsDiskFreeMb, objectName, state, value, false, drive,
                                        advancedSettings, false);
                        }
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.ResourceName))
                {
                    clearValue = refresh.DiskDrives[deviation.MonitoredObject.ResourceName].UnusedSize.Megabytes;
                }

                CheckEvents((int)Metric.OsDiskFreeMb, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null, false);
            }

        }

        private void ProcessAverageDiskMillisecondsPerRead(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry avgDiskSecPerReadThreshold = GetThresholdEntry(Metric.AverageDiskMillisecondsPerRead);
            metricDeviationMap = stateGraph.GetEvents(Metric.AverageDiskMillisecondsPerRead);
            if ((avgDiskSecPerReadThreshold != null) && avgDiskSecPerReadThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(avgDiskSecPerReadThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.AvgDiskSecPerRead != null && drive.AvgDiskSecPerRead.HasValue)
                    {
                        value = drive.AvgDiskSecPerRead.Value.TotalMilliseconds;

                        state = ScheduledRefreshState.GetState(value, avgDiskSecPerReadThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }

                        CheckEvents((int)Metric.AverageDiskMillisecondsPerRead, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    TimeSpan? clearTime =
                        refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].AvgDiskSecPerRead;
                    if (clearTime.HasValue)
                    {
                        clearValue = clearTime.Value.TotalMilliseconds;
                    }
                }

                CheckEvents((int)Metric.AverageDiskMillisecondsPerRead, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null,
                            false);
            }
        }

        private void ProcessAverageDiskMillisecondsPerTransfer(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry avgDiskSecPerTransferThreshold = GetThresholdEntry(Metric.AverageDiskMillisecondsPerTransfer);
            metricDeviationMap = stateGraph.GetEvents(Metric.AverageDiskMillisecondsPerTransfer);
            if ((avgDiskSecPerTransferThreshold != null) && avgDiskSecPerTransferThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(avgDiskSecPerTransferThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.AvgDiskSecPerTransfer != null && drive.AvgDiskSecPerTransfer.HasValue)
                    {
                        value = drive.AvgDiskSecPerTransfer.Value.TotalMilliseconds;

                        state = ScheduledRefreshState.GetState(value, avgDiskSecPerTransferThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }
                        CheckEvents((int)Metric.AverageDiskMillisecondsPerTransfer, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    TimeSpan? clearTime =
                        refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].AvgDiskSecPerTransfer;
                    if (clearTime.HasValue)
                    {
                        clearValue = clearTime.Value.TotalMilliseconds;
                    }
                }

                CheckEvents((int)Metric.AverageDiskMillisecondsPerTransfer, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData,
                            null, false);
            }
        }

        private void ProcessAverageDiskMillisecondsPerWrite(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry avgDiskSecPerWriteThreshold = GetThresholdEntry(Metric.AverageDiskMillisecondsPerWrite);
            metricDeviationMap = stateGraph.GetEvents(Metric.AverageDiskMillisecondsPerWrite);
            if ((avgDiskSecPerWriteThreshold != null) && avgDiskSecPerWriteThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(avgDiskSecPerWriteThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.AvgDiskSecPerWrite != null && drive.AvgDiskSecPerWrite.HasValue)
                    {
                        value = drive.AvgDiskSecPerWrite.Value.TotalMilliseconds;

                        state = ScheduledRefreshState.GetState(value, avgDiskSecPerWriteThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }

                        CheckEvents((int)Metric.AverageDiskMillisecondsPerWrite, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    TimeSpan? clearTime =
                        refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].AvgDiskSecPerWrite;
                    if (clearTime.HasValue)
                    {
                        clearValue = clearTime.Value.TotalMilliseconds;
                    }
                }

                CheckEvents((int)Metric.AverageDiskMillisecondsPerWrite, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null,
                            false);
            }
        }

        private void ProcessDiskReadsPerSecond(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry diskReadsPerSecondThreshold = GetThresholdEntry(Metric.DiskReadsPerSecond);
            metricDeviationMap = stateGraph.GetEvents(Metric.DiskReadsPerSecond);
            if ((diskReadsPerSecondThreshold != null) && diskReadsPerSecondThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(diskReadsPerSecondThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.DiskReadsPerSec != null && drive.DiskReadsPerSec.HasValue)
                    {
                        value = drive.DiskReadsPerSec;

                        state = ScheduledRefreshState.GetState(value, diskReadsPerSecondThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }

                        CheckEvents((int)Metric.DiskReadsPerSecond, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    if (refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskReadsPerSec.HasValue)
                    {
                        clearValue = refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskReadsPerSec.Value;
                    }
                }

                CheckEvents((int)Metric.DiskReadsPerSecond, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null, false);
            }
        }

        private void ProcessDiskTransfersPerSec(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry diskTransfersPerSecondThreshold = GetThresholdEntry(Metric.DiskTransfersPerSecond);
            metricDeviationMap = stateGraph.GetEvents(Metric.DiskTransfersPerSecond);
            if ((diskTransfersPerSecondThreshold != null) && diskTransfersPerSecondThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(diskTransfersPerSecondThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.DiskTransfersPerSec != null && drive.DiskTransfersPerSec.HasValue)
                    {
                        value = drive.DiskTransfersPerSec;

                        state = ScheduledRefreshState.GetState(value, diskTransfersPerSecondThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }
                        CheckEvents((int)Metric.DiskTransfersPerSecond, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    if (refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskTransfersPerSec.HasValue)
                    {
                        clearValue = refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskTransfersPerSec.Value;
                    }
                }

                CheckEvents((int)Metric.DiskTransfersPerSecond, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null, false);
            }
        }

        private void ProcessDiskWritesPerSecond(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry diskWritesPerSecondThreshold = GetThresholdEntry(Metric.DiskWritesPerSecond);
            metricDeviationMap = stateGraph.GetEvents(Metric.DiskWritesPerSecond);
            if ((diskWritesPerSecondThreshold != null) && diskWritesPerSecondThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(diskWritesPerSecondThreshold);

                foreach (DiskDrive drive in refresh.DiskDrives.Values)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              drive.DriveLetter
                                                          };

                    if (drive.DiskWritesPerSec != null && drive.DiskWritesPerSec.HasValue)
                    {
                        value = drive.DiskWritesPerSec;

                        state = ScheduledRefreshState.GetState(value, diskWritesPerSecondThreshold);

                        if (state <= MonitoredState.OK)
                        {
                            continue;
                        }
                        CheckEvents((int)Metric.DiskWritesPerSecond, objectName, state, value, false,
                                    drive, advancedSettings, false);
                    }
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                IComparable clearValue = 0;
                if (refresh.DiskDrives.ContainsKey(deviation.MonitoredObject.AdditionalQualifiers[0]))
                {
                    if (refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskWritesPerSec.HasValue)
                    {
                        clearValue = refresh.DiskDrives[deviation.MonitoredObject.AdditionalQualifiers[0]].DiskWritesPerSec.Value;
                    }
                }

                CheckEvents((int)Metric.DiskWritesPerSecond, deviation.MonitoredObject, MonitoredState.OK, clearValue, false, deviation.AdditionalData, null, false);
            }
        }


        #endregion

        #region log alerts

        private void ProcessLogAlerts(ScheduledRefresh refresh)
        {
            ProcessErrorLog(refresh);
            if (isServerSqlExpress)  //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition 
            {
                if (IsMetricValidForSqlExpress(Metric.AgentLog))
                    ProcessAgentLog(refresh);
            }
            else
                ProcessAgentLog(refresh);
        }

        private void ProcessErrorLog(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;
            var logSizeLimit = new FileSize();

            //Basic log error is dictated by criticality
            MetricThresholdEntry errorLogScanThreshold = GetThresholdEntry(Metric.ErrorLog);
            metricDeviationMap = stateGraph.GetEvents(Metric.ErrorLog);

            // if the alerts are enabled and greatest criticality in the log is greater than ok
            // or the parsing of the log text caused it to be greater than ok
            // or the log size caused it to be in a warning state.
            // If the file is greater than the limit all other log alerts will be skipped
            if ((errorLogScanThreshold != null) && errorLogScanThreshold.IsEnabled
                && refresh.Alerts.ErrorLogMostSevereMatch > MonitoredState.OK)
            {
                advancedSettings = GetAdvancedConfiguration(errorLogScanThreshold);
                logSizeLimit.Kilobytes = advancedSettings.LogSizeLimit;

                int eventCount = 0;
                StringBuilder eventList = new StringBuilder();
                bool hasTruncated = false;

                if (refresh.Alerts.ErrorLogFileSizeInBytes > 0)
                {
                    eventList.Append("The error log has exceeded the size limit. The file has a size of {0} MB.");
                    value = -1;
                }
                else
                {
                    foreach (Triple<MonitoredState, String, bool> logLine in refresh.Alerts.ErrorLogScanLines)
                    {
                        if (!string.IsNullOrEmpty(logLine.Second))
                        {
                            if (logLine.Third)
                            {
                                eventCount++;
                            }
                            int stringLength = eventList.ToString().Length;
                            if (!hasTruncated &&
                                (stringLength >= 1860 || (stringLength + logLine.Second.Length + 9) >= 1860))
                            {
                                eventList.Append("** View the error log for remaining messages **");
                                hasTruncated = true;
                            }
                            if (!hasTruncated)
                            {
                                eventList.Append(logLine.First.ToString());
                                eventList.Append('\t');
                                eventList.AppendLine(logLine.Second);
                            }
                            value = eventCount;
                        }
                    }
                }
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);

                state = refresh.Alerts.ErrorLogMostSevereMatch;

                var logSize = new FileSize();
                logSize.Bytes = refresh.Alerts.ErrorLogFileSizeInBytes;

                CheckEvents((int)Metric.ErrorLog,
                            objectName,
                            state,
                            value,
                            false,
                            new Triple<string, FileSize, FileSize>(eventList.ToString(), logSize, logSizeLimit),
                            advancedSettings,
                            false);

                if (metricDeviationMap.ContainsKey(objectName))
                {
                    metricDeviationMap.Remove(objectName);
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.ErrorLog, deviation.MonitoredObject, MonitoredState.OK, 0, false, new Triple<string, FileSize, FileSize>("** No messages to display **", new FileSize(0), new FileSize(0)), null, false);
            }
        }

        private void ProcessAgentLog(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;
            var logSizeLimit = new FileSize();

            MetricThresholdEntry agentLogScanThreshold = GetThresholdEntry(Metric.AgentLog);
            metricDeviationMap = stateGraph.GetEvents(Metric.AgentLog);

            if ((agentLogScanThreshold != null) && agentLogScanThreshold.IsEnabled
                && refresh.Alerts.AgentLogMostSevereMatch > MonitoredState.OK)
            {
                advancedSettings = GetAdvancedConfiguration(agentLogScanThreshold);
                logSizeLimit.Kilobytes = advancedSettings.LogSizeLimit;

                int eventCount = 0;

                var eventList = new StringBuilder();
                bool hasTruncated = false;

                if (refresh.Alerts.AgentLogFileSizeInBytes > 0)
                {
                    eventList.Append("The agent log has exceeded the size limit. The file has a size of {0} MB.");
                    value = -1;
                }
                else
                {
                    foreach (Triple<MonitoredState, String, bool> logLine in refresh.Alerts.AgentLogScanLines)
                    {
                        if (!string.IsNullOrEmpty(logLine.Second))
                        {
                            if (logLine.Third)
                            {
                                eventCount++;
                            }
                            int stringLength = eventList.ToString().Length;
                            if (!hasTruncated &&
                                (stringLength >= 1860 || (stringLength + logLine.Second.Length + 9) >= 1860))
                            {
                                eventList.Append("** View the error log for remaining messages **");
                                hasTruncated = true;
                            }
                            if (!hasTruncated)
                            {
                                eventList.Append(logLine.First.ToString());
                                eventList.Append('\t');
                                eventList.AppendLine(logLine.Second);
                            }
                            value = eventCount;
                        }
                    }
                }

                var objectName = new MonitoredObjectName(serverName.ServerName);

                state = refresh.Alerts.AgentLogMostSevereMatch;

                var logSize = new FileSize();
                logSize.Bytes = refresh.Alerts.AgentLogFileSizeInBytes;

                CheckEvents((int)Metric.AgentLog,
                            objectName,
                            state,
                            value,
                            false,
                            new Triple<string, FileSize, FileSize>(eventList.ToString(), logSize, logSizeLimit),
                            advancedSettings,
                            false);

                if (metricDeviationMap.ContainsKey(objectName))
                {
                    metricDeviationMap.Remove(objectName);
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                CheckEvents((int)Metric.AgentLog, deviation.MonitoredObject, MonitoredState.OK, 0, false, new Triple<string, FileSize, FileSize>("** No messages to display **", new FileSize(0), new FileSize(0)), null, false);
            }
        }

        #endregion

        #region cluster alerts

        private void ProcessClusterAlerts(ScheduledRefresh refresh)
        {
            ProcessClusterFailover(refresh);
            ProcessClusterActiveNode(refresh);
        }

        private void ProcessClusterFailover(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry clusterFailoverThreshold = GetThresholdEntry(Metric.ClusterFailover);
            metricDeviationMap = stateGraph.GetEvents(Metric.ClusterFailover);
            if ((clusterFailoverThreshold != null) && clusterFailoverThreshold.IsEnabled
                && refresh.Server.IsClustered.HasValue && refresh.Server.IsClustered.Value && isServerAccessible)
            {
                advancedSettings = GetAdvancedConfiguration(clusterFailoverThreshold);

                ScheduledRefresh previousRefresh = workload.PreviousRefresh != null
                                                       ? (ScheduledRefresh)workload.PreviousRefresh
                                                       : null;

                if ((previousRefresh != null && !previousRefresh.CollectionFailed && previousRefresh.Server.IsClustered.HasValue &&
                     previousRefresh.Server.IsClustered.Value
                     && previousRefresh.Server.ClusterNodeName != null && previousRefresh.Server.ClusterNodeName.Length > 0 &&
                     (previousRefresh.Server.ClusterNodeName.ToLowerInvariant() != refresh.Server.ClusterNodeName.ToLowerInvariant()))
                    || (previousRefresh == null && workload.MonitoredServer.ActiveClusterNode != null && workload.MonitoredServer.ActiveClusterNode.Length > 0
                        && workload.MonitoredServer.ActiveClusterNode.ToLowerInvariant() != refresh.Server.ClusterNodeName.ToLowerInvariant())
                    || (previousRefresh != null && previousRefresh.CollectionFailed && workload.MonitoredServer.ActiveClusterNode != null
                        && workload.MonitoredServer.ActiveClusterNode.Length > 0
                        && workload.MonitoredServer.ActiveClusterNode.ToLowerInvariant() != refresh.Server.ClusterNodeName.ToLowerInvariant()))
                {
                    Pair<string, string> additionalData = new Pair<string, string>();

                    if (clusterFailoverThreshold.CriticalThreshold.Enabled)
                    {
                        state = MonitoredState.Critical;
                    }
                    else if (clusterFailoverThreshold.WarningThreshold.Enabled)
                    {
                        state = MonitoredState.Warning;
                    }
                    else
                    {
                        state = MonitoredState.Informational;
                    }

                    additionalData.First = workload.PreviousRefresh == null
                                               ? workload.MonitoredServer.ActiveClusterNode
                                               : (((ScheduledRefresh)workload.PreviousRefresh).Server.ClusterNodeName ??
                                                  previousRefresh.MonitoredServer.ActiveClusterNode);
                    additionalData.Second = refresh.Server.ClusterNodeName;
                    value = 1;

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);

                    CheckEvents((int)Metric.ClusterFailover,
                                objectName,
                                state,
                                value,
                                false,
                                additionalData,
                                advancedSettings,
                                false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            // include previously generated deviations that have not cleared and are less than 12 hours old
            if (advancedSettings == null)
            {
                advancedSettings = GetAdvancedConfiguration(clusterFailoverThreshold);
            }
            DateTime inactiveTime = DateTime.UtcNow - GetFailoverAlertActiveDuration(advancedSettings);
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                if (deviation.LastUpdate >= inactiveTime)
                {
                    if (!deviation.Acknowledged && clusterFailoverThreshold.IsEnabled)
                    {
                        if (clusterFailoverThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (clusterFailoverThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else
                        {
                            state = MonitoredState.Informational;
                        }

                        CheckEvents((int)Metric.ClusterFailover, deviation.MonitoredObject, state, (IComparable)deviation.Value, false, deviation.AdditionalData,
                                    advancedSettings, true);
                    }
                }
                else
                {
                    stateGraph.ClearEvent(deviation);
                }
            }
        }

        private void ProcessClusterActiveNode(ScheduledRefresh refresh)
        {
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
            MonitoredState state = MonitoredState.None;
            IComparable value = null;

            MetricThresholdEntry clusterActiveNodeThreshold = GetThresholdEntry(Metric.ClusterActiveNode);
            metricDeviationMap = stateGraph.GetEvents(Metric.ClusterActiveNode);
            if ((clusterActiveNodeThreshold != null) && clusterActiveNodeThreshold.IsEnabled
                && refresh.Server.IsClustered.HasValue && refresh.Server.IsClustered.Value)
            {
                advancedSettings = GetAdvancedConfiguration(clusterActiveNodeThreshold);

                bool isNull = false;

                if ((workload.MonitoredServer.PreferredClusterNode == null) || (workload.MonitoredServer.PreferredClusterNode.Trim().Length == 0))
                {
                    LOG.Debug("Preferred cluster node is null for " + refresh.ServerName);
                    isNull = true;
                }

                if ((refresh.Server.ClusterNodeName == null) || (refresh.Server.ClusterNodeName.Trim().Length == 0))
                {
                    LOG.Debug("Active cluster node is null for " + refresh.ServerName);
                    isNull = true;
                }

                state = MonitoredState.OK;
                value = state;

                if (!isNull && (workload.MonitoredServer.PreferredClusterNode.ToLowerInvariant() != refresh.Server.ClusterNodeName.ToLowerInvariant()))
                {
                    if (clusterActiveNodeThreshold.CriticalThreshold.Enabled)
                    {
                        state = MonitoredState.Critical;
                    }
                    else if (clusterActiveNodeThreshold.WarningThreshold.Enabled)
                    {
                        state = MonitoredState.Warning;
                    }
                    else if (clusterActiveNodeThreshold.InfoThreshold.Enabled)
                    {
                        state = MonitoredState.Informational;
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    value = state;
                }

                Pair<string, string> additionalData = new Pair<string, string>();
                additionalData.First = refresh.Server.ClusterNodeName;
                additionalData.Second = workload.MonitoredServer.PreferredClusterNode;

                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);

                CheckEvents((int)Metric.ClusterActiveNode, objectName, state, value, false, additionalData, advancedSettings, false);

                if (metricDeviationMap.ContainsKey(objectName))
                {
                    metricDeviationMap.Remove(objectName);
                }
            }
            foreach (MetricState deviation in metricDeviationMap.Values)
            {
                stateGraph.ClearEvent(deviation);
            }
        }

        #endregion


        #endregion

        #region Alert Continuation

        // Continue any alerts of a type not raised by the active refresh
        private void ContinueAlerts()
        {
            foreach (MetricState deviation in stateGraph.GetEvents())
            {
                if (!alertableRefresh.AlertableMetrics.Contains((Metric)deviation.MetricID))
                {
                    //stateGraph.ClearEvent(deviation);
                    var threshold = GetThresholdEntry((Metric)deviation.MetricID);
                    var advancedSettings = GetAdvancedConfiguration(threshold);
                    CheckEvents(deviation.MetricID, deviation.MonitoredObject, deviation.MonitoredState, (IComparable)deviation.Value, false, deviation.AdditionalData,
                                    advancedSettings, true);
                }
            }
        }

        #endregion


        /// <summary>
        /// 	Gets the scheduled refresh events and updates the state graph.
        /// </summary>
        public IEnumerable<IEvent> Process()//  ScheduledRefreshState newState)
        {
            LOG.VerboseFormat("Processing events for {0}", serverName.ServerName);
            isServerAccessible = true;
            if (alertableRefresh == null)
            {
                throw new ArgumentNullException("alertableRefresh");
            }
            if (stateGraph == null)
            {
                throw new ArgumentNullException("stateGraph");
            }

            lock (workload.StateGraph)
            {

                if (alertableRefresh is ScheduledRefresh)
                {
                    LOG.Verbose("Processing ScheduledRefresh");
                    ScheduledRefresh refresh = alertableRefresh as ScheduledRefresh;
                    if (refresh != null)
                    {
                        try
                        {
                            // Process Cloud metric for AWS RDS and Azure SQL Server
                            if (refresh.MonitoredServer.CloudProviderId == Constants.AmazonRDSId)
                            {
                                ProcessAmazonRDSMetrics(refresh);
                            }
                       if (refresh.MonitoredServer.CloudProviderId == Constants.MicrosoftAzureId || refresh.MonitoredServer.CloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                            {
                                ProcessAzureSQLServerMetrics(refresh);
                            }
                        }
                        catch(Exception exception)
                        {
                            LOG.Error("Exception occurred while processing cloud metrics");
                            LOG.Error(exception);
                        }

                        // Process VM alerts whether or not the server is online
                        ProcessVMMetrics(refresh);

                        // Process SQLsafe Metrics
                        //ProcessSQLsafeMetrics(refresh);

                        // SQL Service Status
                        ProcessServerMetric(Metric.SqlServiceStatus, valueAdapter.GetServerValue);

                        //Check whether the server is online before continuing to other alerts
                        isServerAccessible = ServerIsAccessible(refresh);
                        if (!isServerAccessible)
                        {
                            //Re raise Alerts maintained over time 
                            ReRaiseMaintainedOverTimeAlerts(refresh);
                            // if collection failed there is no need to continue
                            return refresh.Events;
                        }

                        ProcessSimpleAlerts(refresh);

                        ProcessDatabaseMetrics(refresh);

                        ProcessDiskAlerts(refresh);

                        ClearNonContinuingAlerts();

                        ProcessComplexAlerts(refresh);

                        ProcessGroomingTimeout();//SQLdm 9.0 -(Ankit Srivastava) - Process newly added grooming timed out metric

                    }
                }
                else if (alertableRefresh is DatabaseSizeSnapshot)
                {
                    LOG.Verbose("Processing DatabaseSizeSnapshot");
                    DatabaseSizeSnapshot dbRefresh = alertableRefresh as DatabaseSizeSnapshot;
                    if (dbRefresh != null)
                    {
                        LOG.Verbose("Starting ProcessServerMetric(Metric.DataUsedPct)");
                        ProcessServerMetric(Metric.DataUsedPct);
                        LOG.Verbose("Starting ProcessServerMetric(Metric.LogUsedPct)");
                        ProcessServerMetric(Metric.LogUsedPct);
                        LOG.Verbose("Starting ProcessDatabaseMetrics(dbRefresh)");
                        ProcessDatabaseMetrics(dbRefresh);

                    }
                }

                // Continue any alerts that are not raised by this refresh type
                //ContinueAlerts();   

            }

            return alertableRefresh.Events;
        }

        //Start - SQLdm 9.0 -(Ankit Srivastava) - Process newly added grooming timed out metric
        private void ProcessGroomingTimeout()
        {
            IManagementService mgmtSvc = RemotingHelper.GetObject<IManagementService>();
            //mgmtSvc.AggregateNow();

            var timedoutInstances = mgmtSvc.GetGroomingTimedOutState(workload.MonitoredServer.Id);

            string[] additionalData = new string[2];
            additionalData[0] = workload.MonitoredServer.InstanceName;
            additionalData[1] = timedoutInstances.Key ? timedoutInstances.Value.Trim() : String.Empty;
            IComparable value = timedoutInstances.Key ? 1 : 0;

            ProcessMetric((int)Metric.RepositoryGroomingTimedOut, serverName, value, additionalData);
        }
        //End - SQLdm 9.0 -(Ankit Srivastava) - Process newly added grooming timed out metric

        private static TimeSpan GetJobAlertsActiveDuration(AdvancedAlertConfigurationSettings advancedSettings)
        {
            return TimeSpan.FromMinutes(advancedSettings != null ? advancedSettings.NumberMinutesJobAlertsAreActive : 720);
        }

        private static TimeSpan GetFailoverAlertActiveDuration(AdvancedAlertConfigurationSettings advancedSettings)
        {
            return TimeSpan.FromMinutes(advancedSettings != null ? advancedSettings.NumberMinutesAgingAlertsAreActive : 720);
        }

        private static TimeSpan GetVMAlertActiveDuration(AdvancedAlertConfigurationSettings advancedSettings)
        {
            return TimeSpan.FromMinutes(advancedSettings != null ? advancedSettings.NumberMinutesAgingAlertsAreActive : 720);
        }

        private void ProcessServerMetric(Metric metric)
        {
            if (isServerSqlExpress)  //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition 
            {
                if (IsMetricValidForSqlExpress(metric))
                    ProcessServerMetric(metric, valueAdapter.GetServerValue);
            }
            else
                ProcessServerMetric(metric, valueAdapter.GetServerValue);
        }

        //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition - starts

        /// <summary>
        /// To check if the given metric is compatible with the Express Edition or not
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        private bool IsMetricValidForSqlExpress(Metric metric)
        {

            bool isValid = false;

            try
            {
                metricCompatibilityForSqlExpress.TryGetValue((int)metric, out isValid);
            }
            catch (Exception ex)
            {
                LOG.Error("Unexpected error in method  IsMetricValidForSqlExpress.");
                LOG.Error(ex);
            }

            return isValid;
        }

        /// <summary>
        /// To check if the current instance is SQL Express Edition or not
        /// </summary>
        /// <returns></returns>
        private bool IsServerSqlExpress()
        {
            bool contains = false;
            try
            {
                if (workload != null && workload.MonitoredServer != null)
                {
                    if (!String.IsNullOrEmpty(workload.MonitoredServer.MostRecentSQLEdition))
                        contains = workload.MonitoredServer.MostRecentSQLEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1;
                    else if (!String.IsNullOrEmpty(productEdition))
                        contains = productEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1;
                    else
                        LOG.Error(string.Format("Could not determine the product edition of Instance:{0}", workload.MonitoredServer.InstanceName));
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Unexpected error in method  IsServerSqlExpress.");
                LOG.Error(ex);
            }
            return contains;
        }

        private void ProcessCloudMetric(Metric metric, object metricval, string dbname = "NA")
        {
            serverName.DatabaseName = dbname;
            ProcessCloudMetric((int)metric, serverName, Convert.ToDouble(metricval), null, metricval);
        }

        //SQLdm 8.6 -- (Ankit Srivastava) -- Stop alert processing which are unnecessary for ExpressEdition - ends

        private void ProcessServerMetric(Metric metric, GetRefreshValue valueDelegate)
        {
            MetricThresholdEntry serverThreshold = GetThresholdEntry(metric);
            IComparable value = valueDelegate(metric, out collectedValue, serverThreshold.IsBaselineEnabled);
            object additionalData = valueAdapter.GetServerAdditionalValue(metric);
            ProcessMetric((int)metric, serverName, value, additionalData, collectedValue);
        }

        private void ProcessServerTempdbMetric(Metric metric)
        {
            ProcessServerTempdbMetric(metric, valueAdapter.GetServerValue);
        }

        private void ProcessServerTempdbMetric(Metric metric, GetRefreshValue valueDelegate)
        {
            MetricThresholdEntry serverThreshold = GetThresholdEntry(metric);
            IComparable value = valueDelegate(metric, collectedValue: out collectedValue);
            object additionalData = valueAdapter.GetServerAdditionalValue(metric);
            MonitoredObjectName database = new MonitoredObjectName(serverName.ServerName, "tempdb");
            ProcessMetric((int)metric, database, value, additionalData, collectedValue);
        }

        private void ProcessMirroringMetrics(ScheduledRefresh refresh)
        {
            LOG.Debug("Entering ProcessMirroringMetrics");

            // if no longer enabled make sure events get cleared
            if (!IsThresholdEnabled(Metric.UnsentLogThreshold))
            {
                ClearEvents(Metric.UnsentLogThreshold);
            }
            if (!IsThresholdEnabled(Metric.UnrestoredLog))
            {
                ClearEvents(Metric.UnrestoredLog);
            }
            if (!IsThresholdEnabled(Metric.OldestUnsentMirroringTran))
            {
                ClearEvents(Metric.OldestUnsentMirroringTran);
            }
            if (!IsThresholdEnabled(Metric.MirrorCommitOverhead))
            {
                ClearEvents(Metric.MirrorCommitOverhead);
            }
            if (!IsThresholdEnabled(Metric.MirroringSessionsStatus))
            {
                ClearEvents(Metric.MirroringSessionsStatus);
            }
            if (!IsThresholdEnabled(Metric.MirroringSessionNonPreferredConfig))
            {
                ClearEvents(Metric.MirroringSessionNonPreferredConfig);
            }
            if (!IsThresholdEnabled(Metric.MirroringSessionRoleChange))
            {
                ClearEvents(Metric.MirroringSessionRoleChange);
            }
            if (!IsThresholdEnabled(Metric.MirroringWitnessConnection))
            {
                ClearEvents(Metric.MirroringWitnessConnection);
            }

            LOG.Debug("ProcessMirroringMetrics - Cleared events");

            AdvancedAlertConfigurationSettings unsentLogAdvanced = null;
            AdvancedAlertConfigurationSettings unrestoredLogAdvanced = null;
            AdvancedAlertConfigurationSettings unsenttransactionsAdvanced = null;
            AdvancedAlertConfigurationSettings mirrorCommitOverheadAdvanced = null;
            AdvancedAlertConfigurationSettings mirrorSessionStatusAdvanced = null;
            AdvancedAlertConfigurationSettings mirrorSessionNonPreferredConfigAdvanced = null;
            AdvancedAlertConfigurationSettings mirrorSessionRoleChangeAdvanced = null;
            AdvancedAlertConfigurationSettings mirrorWitnessConnectionAdvanced = null;


            unsentLogThreshold = GetThresholdEntry(Metric.UnsentLogThreshold);
            if (unsentLogThreshold != null)
            {
                unsentLogAdvanced = GetAdvancedConfiguration(unsentLogThreshold);
            }

            unrestoredLogThreshold = GetThresholdEntry(Metric.UnrestoredLog);
            if (unrestoredLogThreshold != null)
            {
                unrestoredLogAdvanced = GetAdvancedConfiguration(unrestoredLogThreshold);
            }

            unsentTransactionThreshold = GetThresholdEntry(Metric.OldestUnsentMirroringTran);
            if (unsentTransactionThreshold != null)
            {
                unsenttransactionsAdvanced = GetAdvancedConfiguration(unsentTransactionThreshold);
            }

            mirrorCommitOverheadThreshold = GetThresholdEntry(Metric.MirrorCommitOverhead);
            if (mirrorCommitOverheadThreshold != null)
            {
                mirrorCommitOverheadAdvanced = GetAdvancedConfiguration(mirrorCommitOverheadThreshold);
            }

            mirrorSessionStatusThreshold = GetThresholdEntry(Metric.MirroringSessionsStatus);
            if (mirrorSessionStatusThreshold != null)
            {
                mirrorSessionStatusAdvanced = GetAdvancedConfiguration(mirrorSessionStatusThreshold);
            }

            LOG.Debug("ProcessMirroringMetrics - Got advanced config");

            MonitoredObjectName database;
            MonitoredObjectName table;
            MonitoredState state = 0;
            IComparable value = null;

            MonitoredState preferredConfigstate = 0;
            IComparable preferredConfigvalue = null;
            object[] preferredConfigAdditionalData = null;

            //don;t continue if there are no mirrored databases in this refresh
            if (refresh.MirroredDatabases == null)
            {
                return;
            }

            //Go through each mirrored database in the refresh
            foreach (MirrorMonitoringDatabaseDetail mirrorDetail in refresh.MirroredDatabases.Values)
            {
                LOG.Debug(string.Format("Fetching data for {0} on {1}", serverName.ServerName, mirrorDetail.DatabaseName));

                // remove databases from the list of current events as they are processed
                //currentDatabaseEvents.Remove(dbstats.Name);
                database = new MonitoredObjectName(serverName.ServerName, mirrorDetail.DatabaseName);

                ///The following 3 metrics only have meaning on the principal
                if (mirrorDetail.CurrentMirroringMetrics.Role == MirroringMetrics.MirroringRoleEnum.Principal)
                {
                    LOG.Debug(string.Format("Fetching data for principal database {0} on {1}", serverName.ServerName, mirrorDetail.DatabaseName));
                    ProcessMirroredDatabaseMetric(Metric.UnsentLogThreshold, mirrorDetail.CurrentMirroringMetrics,
                                                  database, unsentLogThreshold, unsentLogAdvanced, false);
                    ProcessMirroredDatabaseMetric(Metric.OldestUnsentMirroringTran, mirrorDetail.CurrentMirroringMetrics,
                                                  database, unsentTransactionThreshold, unsenttransactionsAdvanced,
                                                  false);
                    ProcessMirroredDatabaseMetric(Metric.MirrorCommitOverhead, mirrorDetail.CurrentMirroringMetrics,
                                                  database, mirrorCommitOverheadThreshold, mirrorCommitOverheadAdvanced,
                                                  false);
                }

                LOG.Debug(string.Format("Fetching unrestored log {0} on {1}", serverName.ServerName, mirrorDetail.DatabaseName));

                //This alert is good on either the principal or the mirror
                ProcessMirroredDatabaseMetric(Metric.UnrestoredLog, mirrorDetail.CurrentMirroringMetrics, database,
                                              unrestoredLogThreshold, unrestoredLogAdvanced, false);

                //////////////////////////////////////////////
                //The following 3 metrics are state change based
                //////////////////////////////////////////////

                LOG.Debug(string.Format("Getting previous refresh {0} on {1}", serverName.ServerName, mirrorDetail.DatabaseName));

                ScheduledRefresh lastRefresh = null;

                LOG.Debug(string.Format("workload.PreviousRefresh {0} null", workload.PreviousRefresh == null ? "is" : "is not"));

                //if there was a previous refresh
                if (workload.PreviousRefresh != null)
                {
                    //Get the last refresh to see if the role has changed
                    lastRefresh = (ScheduledRefresh)workload.PreviousRefresh;

                    LOG.Debug(string.Format("Previous refresh was at {0}", lastRefresh.TimeStamp));
                }

                mirrorSessionRoleChangeThreshold = GetThresholdEntry(Metric.MirroringSessionRoleChange);
                if (mirrorSessionRoleChangeThreshold != null)
                {
                    mirrorSessionRoleChangeAdvanced = GetAdvancedConfiguration(mirrorSessionRoleChangeThreshold);
                }

                LOG.Debug(string.Format("Got role change advanced settings {0}", mirrorSessionRoleChangeAdvanced == null ? "but it is null" : "and it is valid"));

                IDictionary<MonitoredObjectName, MetricState> deviationMap = stateGraph.GetEvents(Metric.MirroringSessionRoleChange);

                LOG.Debug(string.Format("Got deviation map for session role change."));

                if (mirrorSessionRoleChangeThreshold.IsEnabled && lastRefresh != null)
                {
                    LOG.Debug(string.Format("Alert is enabled and we have a previous refresh."));

                    //MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, mirrorDetail.DatabaseName);
                    MetricState deviation = stateGraph.GetEvent(database, (int)Metric.MirroringSessionRoleChange);

                    LOG.Debug(string.Format("Current role for {0} is {1}", mirrorDetail.DatabaseName, mirrorDetail.CurrentMirroringMetrics.Role));

                    object[] additionalData = new object[]
                                                  {
                                                      mirrorDetail.DatabaseName,
                                                      mirrorDetail.CurrentMirroringMetrics.Role.ToString()
                                                  };

                    state = MonitoredState.OK;
                    value = state;

                    bool blnRoleHasChanged = false;

                    if (lastRefresh.MirroredDatabases.ContainsKey(mirrorDetail.MirroringGuid))
                    {
                        LOG.Debug(string.Format("The last refresh contained role information for {0} when it's role was {1}", mirrorDetail.MirroringGuid,
                                                lastRefresh.MirroredDatabases[mirrorDetail.MirroringGuid].CurrentMirroringMetrics.Role));

                        blnRoleHasChanged = lastRefresh.MirroredDatabases[mirrorDetail.MirroringGuid].
                                                CurrentMirroringMetrics.Role
                                            != mirrorDetail.CurrentMirroringMetrics.Role;

                        LOG.Debug(string.Format("Role {0} changed for {1}", blnRoleHasChanged ? "has" : "has not", mirrorDetail.DatabaseName));
                    }

                    bool blnsetAlert = false;

                    //There is already a deviation
                    if (deviation != null)
                    {
                        LOG.Debug(String.Format("We know we have a role change and we are looking for mirroringSessionRoleChangeAdvanced and it is {0}",
                                                mirrorSessionRoleChangeAdvanced == null ? "null" : "not null"));
                        //if we have past the reqired aging period
                        int intMinutesAgeAlerts = 0;
                        if (mirrorSessionRoleChangeAdvanced != null)
                        {
                            intMinutesAgeAlerts = mirrorSessionRoleChangeAdvanced.NumberMinutesAgingAlertsAreActive;
                        }
                        else
                        {
                            intMinutesAgeAlerts = 720;
                            LOG.Debug(String.Format("There is no advanced role setting so we are going with a default age of {0}", intMinutesAgeAlerts));
                        }

                        if (blnRoleHasChanged)
                        {
                            blnsetAlert = true;
                        }

                        //if the alert is within the aging period
                        if (deviation.StartTime.AddMinutes(intMinutesAgeAlerts).ToLocalTime() < DateTime.Now)
                        {
                            LOG.Debug(string.Format("We have passed the aging period"));

                            //send the ok event
                            state = MonitoredState.OK;
                            //Add a clear here if you want to clear the aged role change alert without it being lowered with a green tick
                        }
                        else
                        {
                            LOG.Debug(string.Format("Alert must clear at {0}", deviation.StartTime.AddMinutes(intMinutesAgeAlerts).ToLocalTime()));


                            //We are still in the aging period. If the role has changed again we need to raise a new alert
                            if (blnRoleHasChanged)
                            {
                                LOG.Debug(string.Format("The role has changed again so we must clear the old alert and raise a new one."));
                                //remove the deviation from the deviationmap to the alert 
                                //shows up as a new one rather than a continuation of the old one
                                if (deviationMap.ContainsKey(database))
                                {
                                    deviationMap.Remove(database);
                                }
                                //clear this event from the stategraph
                                stateGraph.ClearEvent(deviation);
                            }
                            blnsetAlert = true;
                        }
                    }
                    else //There is no existing deviation
                    {
                        LOG.Debug(string.Format("This role change alert is being raised for the first time (or at least it is not already active)"));
                        if (blnRoleHasChanged)
                        {
                            blnsetAlert = true;
                        }
                    }

                    if (blnsetAlert)
                    {
                        LOG.Debug(string.Format("Raising the alert"));
                        //set the state according to the alert thresholds

                        if (mirrorSessionRoleChangeThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (mirrorSessionRoleChangeThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else if (mirrorSessionRoleChangeThreshold.InfoThreshold.Enabled)
                        {
                            state = MonitoredState.Informational;
                        }
                        else
                        {
                            state = MonitoredState.OK;
                        }
                    }

                    value = state;
                    CheckEvents((int)Metric.MirroringSessionRoleChange, database, state, value, false, additionalData,
                                mirrorSessionRoleChangeAdvanced, false);
                    LOG.Debug(string.Format("Checkevents for role change has been performed."));
                }

                //Check if the session is running in conformance to the preferred configuration
                mirrorSessionNonPreferredConfigThreshold = GetThresholdEntry(Metric.MirroringSessionNonPreferredConfig);
                if (mirrorSessionNonPreferredConfigThreshold != null)
                {
                    mirrorSessionNonPreferredConfigAdvanced =
                        GetAdvancedConfiguration(mirrorSessionNonPreferredConfigThreshold);
                }

                LOG.Debug("Got advanced settings for preferred config.");

                if (mirrorSessionNonPreferredConfigThreshold.IsEnabled)
                {
                    //This comes from the workload so if the session has been deleted it will still be here
                    ServerPreferredMirrorConfig preferredConfig =
                        workload.GetMirroringPreferredConfigForServer(mirrorDetail.MirroringGuid);

                    if (preferredConfig == null)
                    {
                        LOG.Debug("Preferred config is null.");
                    }
                    else
                    {
                        LOG.Debug(String.Format("preferredConfig Configuration is enabled and the workload has the role as {0}, witness as {1}", preferredConfig.Role,
                                                preferredConfig.WitnessName));
                    }

                    state = MonitoredState.OK;
                    value = state;

                    // if there is a difference between the preferred role and the current role
                    //or the witness address has changed
                    string Description = "";
                    if (preferredConfig != null && ((preferredConfig.Role != mirrorDetail.CurrentMirroringMetrics.Role)
                                                    || !preferredConfig.WitnessName.Equals(refresh.MirroredDatabases[mirrorDetail.MirroringGuid].WitnessAddress)))
                    {
                        LOG.Debug("Role or witness has changed");

                        if (preferredConfig.Role != mirrorDetail.CurrentMirroringMetrics.Role)
                        {
                            Description = "Role is now " + mirrorDetail.CurrentMirroringMetrics.Role + " instead of " + preferredConfig.Role;
                        }
                        else
                        {
                            Description = "Role is " + mirrorDetail.CurrentMirroringMetrics.Role;
                        }
                        if (!preferredConfig.WitnessName.Equals(refresh.MirroredDatabases[mirrorDetail.MirroringGuid].WitnessAddress))
                        {
                            if (Description.Length > 0)
                            {
                                Description = Description + " and ";
                            }
                            Description = Description + "Witness is " + refresh.MirroredDatabases[mirrorDetail.MirroringGuid].WitnessAddress
                                          + " instead of " + preferredConfig.WitnessName;
                        }

                        if (mirrorSessionNonPreferredConfigThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (mirrorSessionNonPreferredConfigThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else if (mirrorSessionNonPreferredConfigThreshold.InfoThreshold.Enabled)
                        {
                            state = MonitoredState.Informational;
                        }
                        else
                        {
                            state = MonitoredState.OK;
                        }

                        value = state;
                    }
                    else
                    {
                        LOG.Debug("No preferred config deviation.");
                        Description = "Role is " + mirrorDetail.CurrentMirroringMetrics.Role;
                    }

                    object[] additionalData = new object[]
                                                  {
                                                      mirrorDetail.DatabaseName,
                                                      Description
                                                  };

                    CheckEvents((int)Metric.MirroringSessionNonPreferredConfig, database, state, value, false,
                                additionalData, mirrorSessionNonPreferredConfigAdvanced, false);
                    LOG.Debug("Checkevents completed");
                }

                ProcessMirroredDatabaseMetric(Metric.MirroringSessionsStatus, mirrorDetail.CurrentMirroringMetrics,
                                              database, mirrorSessionStatusThreshold, mirrorSessionStatusAdvanced, false);

                LOG.Debug("Processed mirroring sessions status");

                mirrorWitnessConnectionThreshold = GetThresholdEntry(Metric.MirroringWitnessConnection);
                if (mirrorWitnessConnectionThreshold != null)
                {
                    mirrorWitnessConnectionAdvanced = GetAdvancedConfiguration(mirrorWitnessConnectionThreshold);
                }

                LOG.Debug("Witness connection advanced settings");

                MetricState deviation1 = stateGraph.GetEvent(database, (int)Metric.MirroringWitnessConnection);
                if (deviation1 != null)
                {
                    LOG.Debug(string.Format("deviation.value is {0}", deviation1.Value));
                }


                if (mirrorWitnessConnectionThreshold.IsEnabled)
                {
                    state = MonitoredState.OK;
                    value = state;

                    object[] additionalData = new object[]
                                                  {
                                                      mirrorDetail.DatabaseName,
                                                      mirrorDetail.WitnessAddress
                                                  };

                    // if the witness is disconnected
                    if (mirrorDetail.CurrentMirroringMetrics.WitnessStatus ==
                        MirroringMetrics.WitnessStatusEnum.Disconnected)
                    {
                        LOG.Debug("Witness is disconnected");

                        if (mirrorWitnessConnectionThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                            LOG.Debug("Witness is disconnected - raise critical");
                        }
                        else if (mirrorWitnessConnectionThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                            LOG.Debug("Witness is disconnected - raise warning");
                        }
                        else if (mirrorWitnessConnectionThreshold.InfoThreshold.Enabled)
                        {
                            state = MonitoredState.Informational;
                            LOG.Debug("Witness is disconnected - raise informational");
                        }
                        else
                        {
                            state = MonitoredState.OK;
                            LOG.Debug("Witness is disconnected - set to ok");
                        }

                        value = state;
                    }
                    LOG.Debug(String.Format("Witness {0} alert state is {1}", mirrorDetail.WitnessAddress, state));

                    CheckEvents((int)Metric.MirroringWitnessConnection, database, state, value, false, additionalData,
                                mirrorWitnessConnectionAdvanced, false);
                    LOG.Debug("Checkevents completed");
                }
            }

            if (workload.GetPreferredConfig != null)
            {
                IDictionary<Guid, ServerPreferredMirrorConfig> preferredConfigs = workload.GetPreferredConfig;
                //This alert is commented out.  The user can go and set up an alert on the contents of the log file
                //if he really wants to know when a mirror is deleted.

                //Alert on each database that was mirrored but is not any more
                //foreach (KeyValuePair<Guid, ServerPreferredMirrorConfig> sessionKeyValuePair in workload.GetPreferredConfig)
                //{
                //    ServerPreferredMirrorConfig preferredConfig = sessionKeyValuePair.Value;
                //    database = new MonitoredObjectName(serverName.ServerName, preferredConfig.MirroredDatabase);

                //    if (mirrorSessionNonPreferredConfigThreshold.IsEnabled)
                //    {
                //        object[] additionalData = null;
                //        //if the current snapshot no longer contains a preferred role (db is no longer mirrored)
                //        if (!refresh.MirroredDatabases.ContainsKey(sessionKeyValuePair.Key))
                //        {
                //            if (thresholds.TryGetValue((int) Metric.MirroringSessionNonPreferredConfig,
                //                                       out mirrorSessionNonPreferredConfigThreshold))
                //                mirrorSessionNonPreferredConfigAdvanced =
                //                    GetAdvancedConfiguration(mirrorSessionNonPreferredConfigThreshold);

                //            state = MonitoredState.OK;
                //            value = state;

                //            if (mirrorSessionNonPreferredConfigThreshold.CriticalThreshold.Enabled)
                //                state = MonitoredState.Critical;
                //            else if (mirrorSessionNonPreferredConfigThreshold.WarningThreshold.Enabled)
                //                state = MonitoredState.Warning;
                //            else
                //                state = MonitoredState.OK;

                //            value = state;
                //        }
                //        additionalData = new object[]
                //                                 {
                //                                     sessionKeyValuePair.Value.MirroredDatabase,
                //                                     "Mirroring session has been deleted"
                //                                 };
                //        //only bother adding this event if it is more critical that the role and mirror event
                //        //if (preferredConfigstate < state)
                //        //{
                //        //    CheckEvents((int) Metric.MirroringSessionNonPreferredConfig, database, state, value, false,
                //        //                additionalData, mirrorSessionNonPreferredConfigAdvanced, false);
                //       // }

                //    }
                //}
                LOG.Debug("Got the preferred config from the workload");
                //Delete the session from the workload for this server
                if (preferredConfigs != null)
                {
                    Guid[] Guids = new Guid[preferredConfigs.Keys.Count];

                    LOG.Debug(String.Format("Server has {0} preferred configs", preferredConfigs.Keys.Count));

                    ServerPreferredMirrorConfig[] configs = new ServerPreferredMirrorConfig[preferredConfigs.Values.Count];
                    int intCntr = 0;
                    //Copy the dictionary to an array
                    foreach (KeyValuePair<Guid, ServerPreferredMirrorConfig> configKeyValue in preferredConfigs)
                    {
                        Guids[intCntr] = configKeyValue.Key;
                        configs[intCntr] = configKeyValue.Value;
                        intCntr++;
                    }
                    //This nasty hack is because we cannot delete from the dictionary while we enumerate through it
                    for (intCntr = 0; intCntr < preferredConfigs.Keys.Count; intCntr++)
                    {
                        //if the preferred config has a counterpart in the monitored mirrored servers continue
                        if (refresh.MirroredDatabases.ContainsKey(Guids[intCntr]))
                        {
                            continue;
                        }
                        LOG.Debug(String.Format("Guid {0} is in the workload but not in the refresh so delete from the workload", Guids[intCntr]));

                        //if no longer monitored then delete from the dictionary
                        workload.UpdateMirroringSessions(Guids[intCntr], null);
                    }
                }
            }
        }

        private void ProcessMirroredDatabaseMetric(Metric metric, MirroringMetrics metrics, MonitoredObjectName databaseName, MetricThresholdEntry threshold,
                                                   AdvancedAlertConfigurationSettings advanced, bool continued)
        {
            MonitoredState newState = MonitoredState.None;

            if (threshold != null && threshold.IsEnabled)
            {
                IComparable value = valueAdapter.GetMirroredDatabaseValue(metric, metrics);

                if (value != null)
                {
                    newState = ScheduledRefreshState.GetState(value, threshold);
                }

                CheckEvents((int)metric, databaseName, newState, value, false, null, advanced, continued);
            }
        }


        private void ProcessTableFragmentation(ScheduledRefresh refresh)
        {
            MetricThresholdEntry tableReorgThresholdEntry = null;
            AdvancedAlertConfigurationSettings tableReorgAdvanced = null;

            // dig out the database thresholds and their advanced settings
            List<Triple<MonitoredState, TableReorganization, object>> tableReorgList = new List<Triple<MonitoredState, TableReorganization, object>>();
            IDictionary<MonitoredObjectName, MetricState> reorgDeviationMap = stateGraph.GetEvents(Metric.ReorganisationPct);

            MonitoredObjectName database;
            MonitoredObjectName table;
            MonitoredState state;
            IComparable value;

            bool quietTimeRan = //refresh.ReorgRetryCounter == 0 && 
                refresh.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime == refresh.TimeStampLocal;

            //SQLDM-29184
            decimal? tableMinimumSize = refresh.MonitoredServer.TableFragmentationConfiguration.FragmentationMinimumTableSize.Kilobytes;

            // build a map of the databases and tables with outstanding events from the state graph
            Dictionary<string, Set<string>> currentTableEvents = stateGraph.GetDatabaseAndTableMap();
            // build a set of databases with outstanding events
            Set<string> currentDatabaseEvents = new Set<string>(currentTableEvents.Keys);
            //SQLDM-29184
            TableSize tableSize = null;
            foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
            {
                // remove databases from the list of current events as they are processed
                currentDatabaseEvents.Remove(dbstats.Name);

                database = new MonitoredObjectName(serverName.ServerName, dbstats.Name, InstanceType.Database);

                // Table Reorg Metric
                tableReorgThresholdEntry = GetThresholdEntry(Metric.ReorganisationPct);
                if (tableReorgThresholdEntry != null)
                {
                    tableReorgAdvanced = GetAdvancedConfiguration(tableReorgThresholdEntry);
                }

                if (tableReorgThresholdEntry != null && tableReorgThresholdEntry.IsEnabled)
                {
                    tableReorgList.Clear();

                    string[] tableExcludeMatch = null;
                    string[] tableExcludeLike = null;

                    if (tableReorgAdvanced != null)
                    {
                        tableExcludeMatch = tableReorgAdvanced.TableExcludeMatch;
                        tableExcludeLike = tableReorgAdvanced.TableExcludeLike;
                    }

                    bool filterTables = (tableExcludeMatch != null && tableExcludeMatch.Length > 0) ||
                                        (tableExcludeLike != null && tableExcludeLike.Length > 0);

                    foreach (TableReorganization reorg in dbstats.TableReorganizations.Values)
                    {
                        bool hasCurrentEvents = false;
                        // remove tables from the list of current events as they are processed
                        if (currentTableEvents.ContainsKey(dbstats.Name))
                        {
                            hasCurrentEvents = true;
                            Set<string> tables = currentTableEvents[dbstats.Name];
                            tables.Remove(reorg.Name);
                            if (tables.Count == 0)
                            {
                                currentTableEvents.Remove(dbstats.Name);
                            }
                        }

                        // M1: Fragmentation should only be reported on for indexes that are large enough for fragmentation to make a difference in performance.
                        // For most servers, this will be for indexes that are larger than 0.5 % of the memory allocated to SQL Server.
                        // The minimum would be an index that is 8MB in size.
                        table = new MonitoredObjectName(database.ServerName, database.DatabaseName, reorg.Name);
                        //SQLDM-29184
                        dbstats.TableSizes.TryGetValue(reorg.Schema + "." + table.TableName, out tableSize);

                        value = valueAdapter.GetTableReorgValue(Metric.ReorganisationPct, reorg);
                        if (value.CompareTo(Convert.ToDouble(refresh.Server.TargetServerMemory.Megabytes) * 0.5) > 0)
                        {
                            if (value == null)
                            {
                                state = MonitoredState.OK;
                            }
                            else if (tableReorgThresholdEntry.CriticalThreshold.Enabled &&
                                     tableReorgThresholdEntry.CriticalThreshold.IsInViolation(value))
                            {
                                state = MonitoredState.Critical;
                            }
                            else if (tableReorgThresholdEntry.WarningThreshold.Enabled &&
                                     tableReorgThresholdEntry.WarningThreshold.IsInViolation(value))
                            {
                                state = MonitoredState.Warning;
                            }
                            else if (tableReorgThresholdEntry.InfoThreshold.Enabled &&
                                     tableReorgThresholdEntry.InfoThreshold.IsInViolation(value))
                            {
                                state = MonitoredState.Informational;
                            }
                            else
                            {
                                state = MonitoredState.OK;
                            }

                            if (hasCurrentEvents || state > MonitoredState.OK)
                            {
                                if (filterTables && AlertFilterHelper.IsTableExcluded(tableExcludeMatch, tableExcludeLike, dbstats.Name, reorg.Schema, reorg.Name))
                                {
                                    continue;
                                }
                            }
                            tableReorgList.Add(new Triple<MonitoredState, TableReorganization, object>(state, reorg, value));
                        }
                    }
                }
                // summarize the list of tables needing reorganization
                if (tableReorgList.Count > 0)
                {
                    int ok = 0, info = 0, warning = 0, critical = 0;
                    string displayVerb = string.Empty, displayS = string.Empty;
                    StringBuilder criticalBody = new StringBuilder();
                    StringBuilder warningBody = new StringBuilder();
                    StringBuilder infoBody = new StringBuilder();
                    foreach (Triple<MonitoredState, TableReorganization, object> triple in tableReorgList)
                    {
                        switch (triple.First)
                        {
                            case MonitoredState.Critical:
                                critical++;
                                criticalBody.AppendFormat(TABLE_VALUE_FMT,
                                                          triple.Second.FullName, triple.Third);
                                break;
                            case MonitoredState.Warning:
                                warningBody.AppendFormat(TABLE_VALUE_FMT,
                                                         triple.Second.FullName, triple.Third);
                                warning++;
                                break;
                            case MonitoredState.Informational:
                                info++;
                                infoBody.AppendFormat(TABLE_VALUE_FMT,
                                                      triple.Second.FullName, triple.Third);
                                break;
                            case MonitoredState.OK:
                                ok++;
                                break;
                        }
                    }
                    StringBuilder body = new StringBuilder();
                    int threshold =
                        (tableReorgThresholdEntry.InfoThreshold.Enabled)
                            ? (int)Convert.ChangeType(tableReorgThresholdEntry.InfoThreshold.Value, typeof(int))
                            : (tableReorgThresholdEntry.WarningThreshold.Enabled)
                                  ? (int)Convert.ChangeType(tableReorgThresholdEntry.WarningThreshold.Value, typeof(int))
                                  : (int)Convert.ChangeType(tableReorgThresholdEntry.CriticalThreshold.Value, typeof(int));

                    if (critical > 0)
                    {
                        displayVerb = 1.Equals(critical) ? "is" : "are";
                        displayS = 1.Equals(critical) ? string.Empty : "s";
                        threshold =
                            (int)Convert.ChangeType(tableReorgThresholdEntry.CriticalThreshold.Value, typeof(int));
                        body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                          critical, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                        body.Append(criticalBody);
                        body.Append(CRLF);
                        body.Append(CRLF);
                        state = MonitoredState.Critical;
                        value = critical;
                    }
                    else if (warning > 0)
                    {
                        displayVerb = 1.Equals(warning) ? "is" : "are";
                        displayS = 1.Equals(warning) ? string.Empty : "s";
                        threshold =
                            (int)Convert.ChangeType(tableReorgThresholdEntry.WarningThreshold.Value, typeof(int));
                        body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                          warning, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                        body.Append(warningBody);
                        state = MonitoredState.Warning;
                        value = warning;
                    }
                    else if (info > 0)
                    {
                        displayVerb = 1.Equals(info) ? "is" : "are";
                        displayS = 1.Equals(info) ? String.Empty : "s";
                        threshold =
                            (int)Convert.ChangeType(tableReorgThresholdEntry.InfoThreshold.Value, typeof(int));
                        body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                          info, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                        body.Append(infoBody);
                        state = MonitoredState.Informational;
                        value = info;
                    }
                    else
                    {
                        displayVerb = 1.Equals(ok) ? "is" : "are";
                        displayS = 1.Equals(ok) ? string.Empty : "s";
                        body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                          ok, threshold, database.DatabaseName, @"less then", displayVerb, displayS);
                        state = MonitoredState.OK;
                        value = ok;
                    }

                    bool cancelAlert = false;

                    if ((refresh.TableFragmentationStatus == TableFragmentationCollectorStatus.Running ||
                         refresh.TableFragmentationStatus == TableFragmentationCollectorStatus.Stopping)
                        && reorgDeviationMap.ContainsKey(database))
                    {
                        if (reorgDeviationMap[database].MonitoredState == state)
                        {
                            object[] previousReorgAlert = (object[])reorgDeviationMap[database].AdditionalData;
                            value = (int)value + (int)previousReorgAlert[1];
                            string previousBody = previousReorgAlert[6].ToString();
                            previousBody = previousBody.Remove(0, previousBody.IndexOf(CRLF, 0)).Trim(CRLF.ToCharArray());

                            // I cannot find a valid use case where tables would appear twice but cannot root out the re-alerting
                            // So if we are about to double up toss the old data
                            foreach (Triple<MonitoredState, TableReorganization, object> triple in tableReorgList)
                            {
                                if (previousBody.Contains(triple.Second.FullName))
                                {
                                    previousBody = "";
                                    value = (int)value - (int)previousReorgAlert[1];

                                    break;
                                }
                            }

                            displayVerb = 1.Equals((int)value) ? "is" : "are";
                            displayS = 1.Equals((int)value) ? string.Empty : "s";

                            body = new StringBuilder();
                            switch (state)
                            {
                                case MonitoredState.OK:
                                    body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                                      value, threshold, database.DatabaseName, @"less then", displayVerb, displayS);
                                    break;
                                case MonitoredState.Informational:
                                    body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                                      value, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                                    body.Append(infoBody);
                                    body.Append(previousBody);
                                    break;
                                case MonitoredState.Warning:
                                    body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                                      value, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                                    body.Append(warningBody);
                                    body.Append(previousBody);
                                    break;
                                case MonitoredState.Critical:
                                    body.AppendFormat(MULTITABLE_ALERT_VALUE_FMT,
                                                      value, threshold, database.DatabaseName, @"at least", displayVerb, displayS);
                                    body.Append(criticalBody);
                                    body.Append(previousBody);
                                    break;
                            }
                        }
                        else
                        {
                            cancelAlert = (reorgDeviationMap[database].MonitoredState > state);
                        }
                    }

                    //SQLDM-29184
                    if (!cancelAlert)
                    {
                        if(tableSize.DataSize.Kilobytes < tableMinimumSize)
                        {
                            cancelAlert = true;
                        }
                    }
                    if (!cancelAlert)
                    {
                        object[] additionalData = new object[]
                                                      {
                                                          database.ServerName,
                                                          value,
                                                          displayS,
                                                          database.DatabaseName,
                                                          displayVerb,
                                                          threshold,
                                                          body.ToString(),
                                                      };

                        CheckEvents((int)Metric.ReorganisationPct,
                                    database,
                                    state,
                                    value,
                                    false,
                                    additionalData,
                                    tableReorgAdvanced,
                                    false);

                        if (reorgDeviationMap.ContainsKey(database))
                        {
                            reorgDeviationMap.Remove(database);
                        }
                    }
                }
            }

            // If the Table Reorg metric ever supports multi-instance thresholds we will need to re-think how this is done
            if (!quietTimeRan)
            {
                if (tableReorgThresholdEntry != null && tableReorgThresholdEntry.IsEnabled)
                {
                    foreach (MetricState deviation in reorgDeviationMap.Values)
                    {
                        // keep raising these until the next quiet time runs
                        if (!deviation.Acknowledged)
                        {
                            CheckEvents(deviation.MetricID, deviation.MonitoredObject, deviation.MonitoredState, (IComparable)deviation.Value, false,
                                        deviation.AdditionalData, tableReorgAdvanced, true);
                        }
                        string databaseName = deviation.MonitoredObject.DatabaseName;
                        if (currentTableEvents.ContainsKey(databaseName))
                        {
                            currentTableEvents.Remove(databaseName);
                        }
                    }
                }
            }


            // only worry about tables that have been deleted when quiet time collection took place or the database was deleted
            foreach (string databaseName in currentTableEvents.Keys)
            {
                foreach (string tableName in currentTableEvents[databaseName])
                {
                    if (quietTimeRan || currentDatabaseEvents.Contains(databaseName))
                    {
                        ClearDatabaseAlerts(serverName.ServerName, databaseName, tableName, stateGraph);
                    }
                }
            }
        }


        private void ProcessDatabaseMetrics(ScheduledRefresh refresh)
        {
            bool dbStatusEnabled = IsMetricEnabled(Metric.DatabaseStatus);
            bool preferredNodeEnabled = IsMetricEnabled(Metric.PreferredNodeUnavailability); //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature - if the preferred node unavailabilty metric enabled or not

            // if no longer enabled make sure events get cleared
            if (!dbStatusEnabled)
            {
                ClearEvents(Metric.DatabaseStatus);
            }
            //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature - clear the event for preferred node unavailabilty metric
            if (!preferredNodeEnabled)
            {
                ClearEvents(Metric.PreferredNodeUnavailability);
            }


            // build a map of the databases and tables with outstanding events from the state graph
            Dictionary<string, Set<string>> currentTableEvents = stateGraph.GetDatabaseAndTableMap();
            // build a set of databases with outstanding events
            Set<string> currentDatabaseEvents = new Set<string>(currentTableEvents.Keys);

            MonitoredObjectName database;
            MonitoredState state;

            AdvancedAlertConfigurationSettings advancedSettings = null;
            AdvancedAlertConfigurationSettings dbStatusAdvanced = null;
            AdvancedAlertConfigurationSettings preferredNodeAdvanced = null; //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature -get the advanced setting for preferred node unavailabilty metric

            foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
            {
                // remove databases from the list of current events as they are processed
                currentDatabaseEvents.Remove(dbstats.Name);
                database = new MonitoredObjectName(serverName.ServerName, dbstats.Name, InstanceType.Database);

                // Database Status 
                MetricThresholdEntry dbStatusThresholdEntry = GetThresholdEntry(Metric.DatabaseStatus);
                if (dbStatusThresholdEntry != null)
                {
                    dbStatusAdvanced = GetAdvancedConfiguration(dbStatusThresholdEntry);
                }

                // SQLDM-19768, Tolga K, eliminate database status errors when the database is in
                // Readonly intent availability mode
                if ((dbstats.Status & DatabaseStatus.Inaccessible) == 0 ||
                    ((dbstats.Status & DatabaseStatus.ReadOnlyAvailabilityReplica) == 0 &&
                     (dbstats.Status & DatabaseStatus.UnreadableAvailabilityReplica) == 0))
                {
                    ProcessDatabaseMetric(Metric.DatabaseStatus, dbstats, database, dbStatusThresholdEntry, dbStatusAdvanced, false);
                }

                //Process Preferred Node Unavailability
                if (preferredNodeEnabled)  //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature - solved defect DE43810
                {
                    LOG.Verbose("Starting Preferred Node Unavailability");
                    MetricThresholdEntry preferredNodeThresholdEntry = GetThresholdEntry(Metric.PreferredNodeUnavailability, dbstats.Name);
                    if ((preferredNodeThresholdEntry != null) && preferredNodeThresholdEntry.IsThresholdEnabled)
                    {
                        preferredNodeAdvanced = GetAdvancedConfiguration(preferredNodeThresholdEntry);

                        ProcessDatabaseMetric(Metric.PreferredNodeUnavailability, dbstats, database, preferredNodeThresholdEntry,
                                              preferredNodeAdvanced, false);
                    }
                    else
                    {
                        ClearEvents(Metric.PreferredNodeUnavailability, dbstats.Name, InstanceType.Database);
                    }
                }



                // process database autogrow
                IDictionary<MonitoredObjectName, MetricState> metricDeviationMap =
                    stateGraph.GetEvents(Metric.LogFileAutogrow);
                if (dbstats.LogAutogrowDetected)
                {
                    MetricThresholdEntry logAutogrowThreshold = GetThresholdEntry(Metric.LogFileAutogrow);
                    if (logAutogrowThreshold != null && logAutogrowThreshold.IsEnabled)
                    {
                        if (logAutogrowThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else
                        {
                            if (logAutogrowThreshold.WarningThreshold.Enabled)
                            {
                                state = MonitoredState.Warning;
                            }
                            else
                            {
                                state = MonitoredState.Informational;
                            }
                        }

                        advancedSettings = GetAdvancedConfiguration(logAutogrowThreshold);

                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, dbstats.Name);
                        FileSize previousFileSize = new FileSize();

                        if ((workload.PreviousRefresh) != null && ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics != null &&
                            ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics.ContainsKey(dbstats.Name))
                        {
                            previousFileSize = ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics[dbstats.Name].LogFileSize;
                        }

                        CheckEvents((int)Metric.LogFileAutogrow, objectName, state, true, false, previousFileSize, advancedSettings, false);
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                }

                foreach (MetricState deviation in metricDeviationMap.Values)
                {
                    stateGraph.ClearEvent(deviation);
                }


                metricDeviationMap = stateGraph.GetEvents(Metric.DataFileAutogrow);
                if (dbstats.DataAutogrowDetected)
                {
                    MetricThresholdEntry dataAutogrowThreshold = GetThresholdEntry(Metric.DataFileAutogrow);
                    if (dataAutogrowThreshold != null && dataAutogrowThreshold.IsEnabled)
                    {
                        if (dataAutogrowThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else
                        {
                            if (dataAutogrowThreshold.WarningThreshold.Enabled)
                            {
                                state = MonitoredState.Warning;
                            }
                            else
                            {
                                state = MonitoredState.Informational;
                            }
                        }

                        advancedSettings = GetAdvancedConfiguration(dataAutogrowThreshold);

                        MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, dbstats.Name);
                        FileSize previousFileSize = new FileSize();

                        if ((workload.PreviousRefresh) != null && ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics != null &&
                            ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics.ContainsKey(dbstats.Name))
                        {
                            previousFileSize = ((ScheduledRefresh)workload.PreviousRefresh).DbStatistics[dbstats.Name].DataFileSize;
                        }

                        CheckEvents((int)Metric.DataFileAutogrow, objectName, state, true, false, previousFileSize, advancedSettings, false);
                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                }

                foreach (MetricState deviation in metricDeviationMap.Values)
                {
                    stateGraph.ClearEvent(deviation);
                }
            }


            // process any databases that no longer exist
            foreach (string databaseName in currentDatabaseEvents)
            {
                ClearDatabaseAlerts(serverName.ServerName, databaseName, null, stateGraph);
            }
        }


        private void ProcessDatabaseMetrics(DatabaseSizeSnapshot dbRefresh)
        {
            //SQLdm 10.0 : (Sanjali Makkar) : Small Features : Updating '# databases' counter
            Statistics.SetDatabases(dbRefresh.DatabaseSummary.DatabaseCount, serverName.ServerName);

            using (LOG.VerboseCall("ProcessDatabaseMetrics"))
            {
                bool dbStatusEnabled = IsMetricEnabled(Metric.DatabaseStatus);
                bool dbSizePctEnabled = IsMetricEnabled(Metric.DatabaseSizePct);
                bool logSizePctEnabled = IsMetricEnabled(Metric.TransLogSize);
                bool reorgPctEnabled = IsMetricEnabled(Metric.ReorganisationPct);
                bool dbSizeMbEnabled = IsMetricEnabled(Metric.DatabaseSizeMb);
                bool logSizeMbEnabled = IsMetricEnabled(Metric.TransLogSizeMb);
                //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --initialize the metric
                bool filegroupSpaceFullPctEnabled = IsMetricEnabled(Metric.FilegroupSpaceFullPct);
                bool filegroupSpaceFullSizeEnabled = IsMetricEnabled(Metric.FilegroupSpaceFullSize);
                //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --initialize the metric
                // SQLdm 10.0 (Vandana Gogna) - Database backup alerts
                bool lastBackupDatabaseEnabled = IsMetricEnabled(Metric.DatabaseLastBackupDate);
                LOG.Verbose("Clearing Events");
                // if no longer enabled make sure events get cleared);
                if (!dbStatusEnabled)
                {
                    ClearEvents(Metric.DatabaseStatus);
                }
                if (!dbSizePctEnabled)
                {
                    ClearEvents(Metric.DatabaseSizePct);
                }
                if (!logSizePctEnabled)
                {
                    ClearEvents(Metric.TransLogSize);
                }
                if (!reorgPctEnabled)
                {
                    ClearEvents(Metric.ReorganisationPct);
                }
                if (!dbSizeMbEnabled)
                {
                    ClearEvents(Metric.DatabaseSizeMb);
                }
                if (!logSizeMbEnabled)
                {
                    ClearEvents(Metric.TransLogSizeMb);
                }

                //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --clear events if no longer enabled
                if (!filegroupSpaceFullPctEnabled)
                {
                    ClearEvents(Metric.FilegroupSpaceFullPct);
                }
                if (!filegroupSpaceFullSizeEnabled)
                {
                    ClearEvents(Metric.FilegroupSpaceFullSize);
                }
                //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --clear events if no longer enabled

                if (!lastBackupDatabaseEnabled)
                {
                    ClearEvents(Metric.DatabaseLastBackupDate);
                }

                AdvancedAlertConfigurationSettings dbStatusAdvanced = null;
                AdvancedAlertConfigurationSettings dbSizeAdvanced = null;
                AdvancedAlertConfigurationSettings logSizeAdvanced = null;

                AdvancedAlertConfigurationSettings dbSizeMbAdvanced = null;
                AdvancedAlertConfigurationSettings logSizeMbAdvanced = null;

                //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --initialize the advanced alert configuration
                AdvancedAlertConfigurationSettings filegroupSpaceFullPctAdvanced = null;
                AdvancedAlertConfigurationSettings filegroupSpaceFullSizeAdvanced = null;
                //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --initialize the advanced alert configuration

                AdvancedAlertConfigurationSettings lastBackupDatabaseAdvanced = null;

                LOG.Verbose("Finished clearing events");
                // build a map of the databases and tables with outstanding events from the state graph
                Dictionary<string, Set<string>> currentTableEvents = stateGraph.GetDatabaseAndTableMap();
                // build a set of databases with outstanding events
                Set<string> currentDatabaseEvents = new Set<string>(currentTableEvents.Keys);

                MonitoredObjectName database;

                LOG.Verbose("Starting foreach DatabaseStatistics");
                foreach (DatabaseStatistics dbstats in dbRefresh.DbStatistics.Values)
                {
                    // remove databases from the list of current events as they are processed
                    currentDatabaseEvents.Remove(dbstats.Name);
                    database = new MonitoredObjectName(serverName.ServerName, dbstats.Name, InstanceType.Database);

                    // Database Status 
                    MetricThresholdEntry dbStatusThresholdEntry = GetThresholdEntry(Metric.DatabaseStatus);
                    if (dbStatusThresholdEntry != null)
                    {
                        LOG.Verbose("Starting Database Status");
                        dbStatusAdvanced = GetAdvancedConfiguration(dbStatusThresholdEntry);
                    }

                    // SQLDM-19768, Tolga K, eliminate database status errors when the database is in
                    // Readonly intent availability mode
                    if ((dbstats.Status & DatabaseStatus.Inaccessible) == 0 ||
                        ((dbstats.Status & DatabaseStatus.ReadOnlyAvailabilityReplica) == 0 &&
                         (dbstats.Status & DatabaseStatus.UnreadableAvailabilityReplica) == 0))
                    {
                        ProcessDatabaseMetric(Metric.DatabaseStatus, dbstats, database, dbStatusThresholdEntry, dbStatusAdvanced, false);
                    }

                    // Database Size (percent)
                    if (dbSizePctEnabled)
                    {
                        LOG.Verbose("Starting Database Size");
                        MetricThresholdEntry dbSizeThresholdEntry = GetThresholdEntry(Metric.DatabaseSizePct, dbstats.Name);
                        if ((dbSizeThresholdEntry != null) && dbSizeThresholdEntry.IsThresholdEnabled)
                        {
                            dbSizeAdvanced = GetAdvancedConfiguration(dbSizeThresholdEntry);

                            ProcessDatabaseMetric(Metric.DatabaseSizePct, dbstats, database, dbSizeThresholdEntry,
                                                  dbSizeAdvanced, false);
                        }
                        else
                        {
                            ClearEvents(Metric.DatabaseSizePct, dbstats.Name, InstanceType.Database);
                        }
                    }

                    // Log Size (percent)
                    if (logSizePctEnabled)
                    {
                        LOG.Verbose("Starting Log Size");
                        MetricThresholdEntry logSizeThresholdEntry = GetThresholdEntry(Metric.TransLogSize, dbstats.Name);
                        if ((logSizeThresholdEntry != null) && logSizeThresholdEntry.IsThresholdEnabled)
                        {
                            logSizeAdvanced = GetAdvancedConfiguration(logSizeThresholdEntry);

                            ProcessDatabaseMetric(Metric.TransLogSize, dbstats, database, logSizeThresholdEntry,
                                                  logSizeAdvanced,
                                                  false);
                        }
                        else
                        {
                            ClearEvents(Metric.TransLogSize, dbstats.Name, InstanceType.Database);
                        }
                    }

                    // Database Size (MB)
                    if (dbSizeMbEnabled)
                    {
                        LOG.Verbose("Starting Database Size (MB)");
                        MetricThresholdEntry dbSizeMbThresholdEntry = GetThresholdEntry(Metric.DatabaseSizeMb, dbstats.Name);
                        if ((dbSizeMbThresholdEntry != null) && dbSizeMbThresholdEntry.IsThresholdEnabled)
                        {
                            dbSizeMbAdvanced = GetAdvancedConfiguration(dbSizeMbThresholdEntry);

                            ProcessDatabaseMetric(Metric.DatabaseSizeMb, dbstats, database, dbSizeMbThresholdEntry,
                                                  dbSizeMbAdvanced, false);
                        }
                        else
                        {
                            ClearEvents(Metric.DatabaseSizeMb, dbstats.Name, InstanceType.Database);
                        }
                    }

                    // Log Size (MB)
                    if (logSizeMbEnabled)
                    {
                        LOG.Verbose("Starting Log Size (MB)");
                        MetricThresholdEntry logSizeMbThresholdEntry = GetThresholdEntry(Metric.TransLogSizeMb, dbstats.Name);
                        if ((logSizeMbThresholdEntry != null) && logSizeMbThresholdEntry.IsThresholdEnabled)
                        {
                            logSizeMbAdvanced = GetAdvancedConfiguration(logSizeMbThresholdEntry);

                            ProcessDatabaseMetric(Metric.TransLogSizeMb, dbstats, database, logSizeMbThresholdEntry,
                                                  logSizeMbAdvanced, false);
                        }
                        else
                        {
                            ClearEvents(Metric.TransLogSizeMb, dbstats.Name, InstanceType.Database);
                        }
                    }

                    //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added new metrices for Filegroup space full alerts
                    //FilegroupSpaceFullPct
                    if (filegroupSpaceFullPctEnabled)
                    {
                        LOG.Verbose("Starting Filegroup Space Full (Percent)");
                        MetricThresholdEntry filegroupSpaceFullPctThresholdEntry = GetThresholdEntry(Metric.FilegroupSpaceFullPct, dbstats.Name);
                        if ((filegroupSpaceFullPctThresholdEntry != null) && filegroupSpaceFullPctThresholdEntry.IsThresholdEnabled)
                        {
                            filegroupSpaceFullPctAdvanced = GetAdvancedConfiguration(filegroupSpaceFullPctThresholdEntry);

                            ProcessDatabaseMetric(Metric.FilegroupSpaceFullPct, dbstats, database, filegroupSpaceFullPctThresholdEntry,
                                                  filegroupSpaceFullPctAdvanced, false);
                        }
                        else
                        {
                            ClearEvents(Metric.FilegroupSpaceFullPct, dbstats.Name, InstanceType.Database);
                        }
                    }

                    //FilegroupSpaceFullSize 
                    if (filegroupSpaceFullSizeEnabled)
                    {
                        LOG.Verbose("Starting Filegroup Space Full (Size)");
                        MetricThresholdEntry filegroupSpaceFullSizeThresholdEntry = GetThresholdEntry(Metric.FilegroupSpaceFullSize, dbstats.Name);
                        if ((filegroupSpaceFullSizeThresholdEntry != null) && filegroupSpaceFullSizeThresholdEntry.IsThresholdEnabled)
                        {
                            filegroupSpaceFullSizeAdvanced = GetAdvancedConfiguration(filegroupSpaceFullSizeThresholdEntry);

                            ProcessDatabaseMetric(Metric.FilegroupSpaceFullSize, dbstats, database, filegroupSpaceFullSizeThresholdEntry,
                                                  filegroupSpaceFullSizeAdvanced, false);
                        }
                        else
                        {
                            ClearEvents(Metric.FilegroupSpaceFullSize, dbstats.Name, InstanceType.Database);
                        }
                    }
                    //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added new metrices for Filegroup space full alerts

                    // Database Last backed up since
                    if (lastBackupDatabaseEnabled && SafeOperationBackupAlerts(dbstats.Status))
                    {
                        LOG.Verbose("Starting Last Backup Database");
                        MetricThresholdEntry lastBackupDatabaseThresholdEntry = GetThresholdEntry(Metric.DatabaseLastBackupDate, dbstats.Name);
                        if ((lastBackupDatabaseThresholdEntry != null) && lastBackupDatabaseThresholdEntry.IsThresholdEnabled)
                        {
                            lastBackupDatabaseAdvanced = GetAdvancedConfiguration(lastBackupDatabaseThresholdEntry);
                            //SQLdm 10.0 (Gaurav Karwal): tempdb never gets backed up
                            if (string.CompareOrdinal(database.DatabaseName, "tempdb") != 0)
                            {
                                // M1
                                // It is not necessary to check the days since last backup if the database is in one of the following states
                                if (!(dbstats.Status == DatabaseStatus.Offline ||
                                        dbstats.Status == DatabaseStatus.Recovering ||
                                        dbstats.Status == DatabaseStatus.Standby ||
                                        dbstats.Status == DatabaseStatus.RestoringMirror))
                                {
                                    ProcessDatabaseMetric(Metric.DatabaseLastBackupDate, dbstats, database, lastBackupDatabaseThresholdEntry,
                                                          lastBackupDatabaseAdvanced, false);
                                }
                            }
                        }
                        else
                        {
                            ClearEvents(Metric.DatabaseLastBackupDate, dbstats.Name, InstanceType.Database);
                        }
                    }
                }

                LOG.Verbose("Clearing out of date databases");
                // process any databases that no longer exist
                foreach (string databaseName in currentDatabaseEvents)
                {
                    ClearDatabaseAlerts(serverName.ServerName, databaseName, null, stateGraph);
                }
            }
        }

        /// <summary>
        /// Verify if a database is part of Availability group, Mirroring or is a snapshot
        /// </summary>
        /// <param name="isPrimary">Availability Group role.</param>
        /// <param name="dbStatus">Database status </param>
        private bool SafeOperationBackupAlerts(bool? isPrimary, DatabaseStatus dbStatus)
        {
            bool primaryAg = (isPrimary == null || isPrimary == true);
            bool mirroring = (dbStatus & DatabaseStatus.RestoringMirror) != DatabaseStatus.RestoringMirror;
            //bool snapshotDb = (dbStatus & DatabaseStatus.ReadOnly) != DatabaseStatus.ReadOnly;


            // SQLdm 10.1.3 - Varun Chopra -- SQLDM-25442 -- allow alert for databases that are online

            //SQLDM-29210. Allow Alerts fro Read-only databases.
			return primaryAg && mirroring ;
        }

        /// <summary>
        /// An overload of SafeOperationBackupAlerts, added by Tolga K for SQLDM-25476
        /// Replaced isPrimary argument with true to avoid breaking of "days since last backup" metric
        /// when target database is in an Availability Group
        /// </summary>
        /// <param name="dbStatus">Database status </param>
        private bool SafeOperationBackupAlerts(DatabaseStatus dbStatus)
        {
            return SafeOperationBackupAlerts(true, dbStatus);
        }

        private void ProcessSQLsafeOperationMetrics(ScheduledRefresh refresh)
        {
            // Check to see if Metircs are enabled
            var ssBackupEnabled = IsMetricEnabled(Metric.SSBackupOperation);
            var ssRestoreEnabled = IsMetricEnabled(Metric.SSRestoreOperation);
            var ssDefragEnabled = IsMetricEnabled(Metric.SSDefragOperation);

            // If metric is disabled, clear any active alerts
            if (!ssBackupEnabled)
                ClearEvents(Metric.SSBackupOperation);
            if (!ssRestoreEnabled)
                ClearEvents(Metric.SSRestoreOperation);
            if (!ssDefragEnabled)
                ClearEvents(Metric.SSDefragOperation);
        }

        private void ProcessAzureSQLServerMetrics(ScheduledRefresh refresh)
        {
            if (refresh.AzureCloudMetrics != null)
            {
                foreach (var dbMetrics in refresh.AzureCloudMetrics)
                {
                    string currentAzureDBName = dbMetrics.Key;
                    foreach (var metric in dbMetrics.Value)
                    {
                        switch (metric.Key)
                        {
                            case "AverageDataIOPercent":
                                ProcessCloudMetric(Metric.AverageDataIOPercent, metric.Value, currentAzureDBName);
                                break;
                            case "AverageLogWritePercent":
                                ProcessCloudMetric(Metric.AverageLogWritePercent, metric.Value, currentAzureDBName);
                                break;
                            case "MaxWorkerPercent":
                                ProcessCloudMetric(Metric.MaxWorkerPercent, metric.Value, currentAzureDBName);
                                break;
                            case "MaxSessionPercent":
                                ProcessCloudMetric(Metric.MaxSessionPercent, metric.Value, currentAzureDBName);
                                break;
                            case "ServiceTierChanges":
                                ProcessCloudMetric(Metric.ServiceTierChanges, metric.Value, currentAzureDBName);
                                break;
                            case "DatabaseAverageMemoryUsagePercent":
                                ProcessCloudMetric(Metric.DatabaseAverageMemoryUsagePercent, metric.Value, currentAzureDBName);
                                break;
                            case "InMemoryStorageUsagePercent":
                                ProcessCloudMetric(Metric.InMemoryStorageUsagePercent, metric.Value, currentAzureDBName);
                                break;
                            //START 5.4.2
                            case "AverageDataIOPercentLow":
                                ProcessCloudMetric(Metric.AverageDataIOPercentLow, metric.Value, currentAzureDBName);
                                break;
                            case "AverageLogWritePercentLow":
                                ProcessCloudMetric(Metric.AverageLogWritePercentLow, metric.Value, currentAzureDBName);
                                break;
                            case "MaxWorkerPercentLow":
                                ProcessCloudMetric(Metric.MaxWorkerPercentLow, metric.Value, currentAzureDBName);
                                break;
                            case "MaxSessionPercentLow":
                                ProcessCloudMetric(Metric.MaxSessionPercentLow, metric.Value, currentAzureDBName);
                                break;
                            case "DatabaseAverageMemoryUsagePercentLow":
                                ProcessCloudMetric(Metric.DatabaseAverageMemoryUsagePercentLow, metric.Value, currentAzureDBName);
                                break;
                            case "InMemoryStorageUsagePercentLow":
                                ProcessCloudMetric(Metric.InMemoryStorageUsagePercentLow, metric.Value, currentAzureDBName);
                                break;
                            //END 5.4.2
                            default:
                                break;
                        }
                    }
                }
            }
        }


        private void ProcessAmazonRDSMetrics(ScheduledRefresh refresh)
        {
            if (refresh.AWSCloudMetrics != null)
            {
                foreach (var metric in refresh.AWSCloudMetrics)
                {
                    switch (metric.Key)
                    {
                        case "CPUCreditBalance":
                            ProcessCloudMetric(Metric.CPUCreditBalance, metric.Value);
                            break;
                        case "CPUCreditBalanceHigh":
                            ProcessCloudMetric(Metric.CPUCreditBalanceHigh, metric.Value);
                            break;
                        case "CPUCreditUsage":
                            ProcessCloudMetric(Metric.CPUCreditUsage, metric.Value);
                            break;
                        case "DiskQueueDepth":
                            ProcessCloudMetric(Metric.DiskQueueDepth, metric.Value);
                            break;
                        case "ReadLatency":
                            ProcessCloudMetric(Metric.ReadLatency, metric.Value);
                            break;
                        case "ReadLatencyLow":
                            ProcessCloudMetric(Metric.ReadLatencyLow, metric.Value);
                            break;
                        case "ReadThroughput":
                            ProcessCloudMetric(Metric.ReadThroughput, metric.Value);
                            break;
                        case "SwapUsage":
                            ProcessCloudMetric(Metric.SwapUsage, metric.Value);
                            break;
                        case "WriteLatency":
                            ProcessCloudMetric(Metric.WriteLatency, metric.Value);
                            break;
                        case "WriteLatencyLow":
                            ProcessCloudMetric(Metric.WriteLatencyLow, metric.Value);
                            break;
                        case "WriteThroughput":
                            ProcessCloudMetric(Metric.WriteThroughput, metric.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal static AdvancedAlertConfigurationSettings GetAdvancedConfiguration(MetricThresholdEntry entry)
        {
            AdvancedAlertConfigurationSettings settings = null;
            if (entry.Data != null)
            {
                settings = entry.Data as AdvancedAlertConfigurationSettings;
                if (settings == null)
                {
                    settings = new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(entry.MetricID), entry.Data);
                }
            }
            else
            {
                settings = new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(entry.MetricID), entry.Data);
            }
            return settings;
        }


        private void ProcessDatabaseMetric(Metric metric, DatabaseStatistics dbstats, MonitoredObjectName databaseName, MetricThresholdEntry threshold,
                                          AdvancedAlertConfigurationSettings advanced, bool continued)
        {
            MonitoredState newState = MonitoredState.None;

            if (threshold != null && threshold.IsEnabled)
            {
                // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --Checking the values for each filegroup in the given database
                Dictionary<string, double> fileGroupsValues = null;
                if (metric == Metric.FilegroupSpaceFullPct || metric == Metric.FilegroupSpaceFullSize)
                {
                    fileGroupsValues = valueAdapter.GetDatabaseFilegroupValues(metric, dbstats, advanced);

                    if (fileGroupsValues != null)
                    {
                        foreach (string filegroup in fileGroupsValues.Keys)
                        {
                            IComparable metricValue = fileGroupsValues[filegroup];

                            if (metricValue != null)
                            {
                                newState = ScheduledRefreshState.GetState(metricValue, threshold);
                            }
                            databaseName.Filegroup = filegroup;
                            CheckEvents((int)metric, databaseName, newState, metricValue, false, new object[] { dbstats.ServerName, fileGroupsValues[filegroup], dbstats.Name, metric, filegroup }, advanced, continued);
                        }

                        //Clean up filegroup for the next metric to be evaluated, in this case it is 'last backup database'.
                        databaseName.Filegroup = null;
                    }
                }
                // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --Checking the values for each filegroup in the given database
                else
                {
                    IComparable value = valueAdapter.GetDatabaseValue(metric, dbstats, advanced);

                    if (value != null)
                    {
                        newState = ScheduledRefreshState.GetState(value, threshold);
                    }

                    //SQLdm 8.6 (Ankit Srivastava) - Preferred Node Feature - added additional data for this metric
                    if (metric == Metric.PreferredNodeUnavailability)
                    {
                        if (!String.IsNullOrEmpty(advanced.ReplicaInstanceName))
                            CheckEvents((int)metric, databaseName, newState, value, false, new object[] { advanced.ReplicaInstanceName, metric, dbstats.Name, advanced.SuppressionPeriodMinutes }, advanced, continued);
                    }
                    else
                        CheckEvents((int)metric, databaseName, newState, value, false, null, advanced, continued);
                }
            }
        }



        private void ProcessCloudMetric(int metricID, MonitoredObjectName target, IComparable value, object additionalData, object metricValue = null)
        {
            MetricThresholdEntry thresholdEntry = GetThresholdEntry(metricID);
            // see if the threshold is enabled
            if (thresholdEntry != null && thresholdEntry.IsEnabled)
            {
                AdvancedAlertConfigurationSettings advanced = GetAdvancedConfiguration(thresholdEntry);

                MonitoredState newState = MonitoredState.None;
                // check the collected value against the threshold to get the state
                if (value != null)
                {


                    switch (metricID)
                    {
                        case (int)Metric.AverageDataIOPercentLow:
                        case (int)Metric.AverageLogWritePercentLow:
                        case (int)Metric.MaxWorkerPercentLow:
                        case (int)Metric.MaxSessionPercentLow:
                        case (int)Metric.DatabaseAverageMemoryUsagePercentLow:
                        case (int)Metric.InMemoryStorageUsagePercentLow:
                        case (int)Metric.ReadLatencyLow:
                        case (int)Metric.WriteLatencyLow:
                            if (thresholdEntry.InfoThreshold.IsInViolation(value))
                            {
                                newState = MonitoredState.Informational;
                            }
                            break;
                        default:
                            newState = ScheduledRefreshState.GetState(value, thresholdEntry);
                            break;

                    }
                }
                

                // check the new state against the state graph and generate necessary events
                CheckEvents(metricID, target, newState, value, false, additionalData, advanced, false, thresholdEntry.IsBaselineEnabled, metricValue);
            }
            else
            {
                if (thresholdEntry == null)
                {
                    LOG.WarnFormat("ProcessCloudMetric() could not retrieve the default threshold entry for metric [{0}]", metricID);
                }
                else
                {
                    // make sure state gets cleared for disabled metrics
                    ClearEvents(metricID);
                }
            }
        }

        private void ProcessMetric(int metricID, MonitoredObjectName target, IComparable value, object additionalData, object metricValue = null)
        {
            MetricThresholdEntry thresholdEntry = GetThresholdEntry(metricID);
            // see if the threshold is enabled
            if (thresholdEntry != null && thresholdEntry.IsEnabled)
            {
                AdvancedAlertConfigurationSettings advanced = GetAdvancedConfiguration(thresholdEntry);

                MonitoredState newState = MonitoredState.None;
                // check the collected value against the threshold to get the state
                if (value != null)
                {
                    newState = ScheduledRefreshState.GetState(value, thresholdEntry);
                }

                // check the new state against the state graph and generate necessary events
                CheckEvents(metricID, target, newState, value, false, additionalData, advanced, false, thresholdEntry.IsBaselineEnabled, metricValue);
            }
            else
            {
                if (thresholdEntry == null)
                {
                    LOG.WarnFormat("ProcessMetric() could not retrieve the default threshold entry for metric [{0}]", metricID);
                }
                else
                {
                    // make sure state gets cleared for disabled metrics
                    ClearEvents(metricID);
                }
            }
        }

        private void ProcessVirtualizationMetrics(ScheduledRefresh refresh)
        {
            try
            {
                MonitoredState state = MonitoredState.None;
                IComparable value = null;
                MetricThresholdEntry vmThreshold = null;
                AdvancedAlertConfigurationSettings vmAdvancedSettings = null;
                IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = null;
                DateTime alertDuration = DateTime.UtcNow - TimeSpan.FromHours(12);
                ScheduledRefresh previousRefresh = workload.PreviousRefresh != null
                                                       ? (ScheduledRefresh)workload.PreviousRefresh
                                                       : null;
                bool vmPoweredOn = false;
                bool esxPoweredOn = false;
                var previousVmConfig = (previousRefresh != null && previousRefresh.Server != null) ? previousRefresh.Server.VMConfig : null;

                if (refresh.Server == null || refresh.Server.VMConfig == null)
                {
                    LOG.Warn("Server or VMConfig was null in ProcessVirtualizationMetrics. Skipping VM alerting.");
                    return;
                }

                // VM Power State
                value = refresh.Server.VMConfig.Status;

                ProcessMetric((int)Metric.VmPowerState, serverName, value, refresh.Server.VMConfig);
                vmPoweredOn = (refresh.Server.VMConfig.Status == VirtualMachinePowerState.poweredOn);

                // ESX Power State
                if (refresh.Server.VMConfig.ESXHost != null)
                {
                    value = refresh.Server.VMConfig.ESXHost.Status;

                    ProcessMetric((int)Metric.VmESXPowerState, serverName, value, refresh.Server.VMConfig);
                    esxPoweredOn = (refresh.Server.VMConfig.ESXHost.Status == HostSystemPowerState.poweredOn);
                }
                else
                {
                    LOG.Warn("ESXHost was null in ProcessVirtualizationMetrics()");
                }

                // VM Config Change
                metricDeviationMap = stateGraph.GetEvents(Metric.VmConfigChange);

                vmThreshold = GetThresholdEntry(Metric.VmConfigChange);
                if ((vmThreshold != null) && vmThreshold.IsEnabled)
                {
                    if (previousVmConfig != null)
                    {
                        // If hostname has changed this is a host move, not a config change
                        if (previousVmConfig.HostName == refresh.Server.VMConfig.HostName)
                        {
                            bool hasChanged = false;
                            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);

                            hasChanged = previousVmConfig.NumCPUs != refresh.Server.VMConfig.NumCPUs;

                            if (!hasChanged)
                            {
                                hasChanged = previousVmConfig.CPULimit != refresh.Server.VMConfig.CPULimit;
                            }

                            if (!hasChanged)
                            {
                                hasChanged = previousVmConfig.CPUReserve != refresh.Server.VMConfig.CPUReserve;
                            }
                            if (!hasChanged)
                            {
                                // If neither is null, compare values
                                if (previousVmConfig.MemSize != null && refresh.Server.VMConfig.MemSize != null)
                                {
                                    hasChanged = previousVmConfig.MemSize.Bytes != refresh.Server.VMConfig.MemSize.Bytes;
                                }
                                else
                                {
                                    // If one is null, it's a change. If both are null, it's not.
                                    hasChanged = !(previousVmConfig.MemSize == null && previousVmConfig.MemSize == null);
                                }
                            }
                            if (!hasChanged)
                            {
                                // If neither is null, compare values
                                if (previousVmConfig.MemLimit != null && refresh.Server.VMConfig.MemLimit != null)
                                {
                                    hasChanged = previousVmConfig.MemLimit.Bytes != refresh.Server.VMConfig.MemLimit.Bytes;
                                }
                                else
                                {
                                    // If one is null, it's a change. If both are null, it's not.
                                    hasChanged = !(previousVmConfig.MemLimit == null && previousVmConfig.MemLimit == null);
                                }
                            }
                            if (!hasChanged)
                            {
                                // If neither is null, compare values
                                if (previousVmConfig.MemReserve != null && refresh.Server.VMConfig.MemReserve != null)
                                {
                                    hasChanged = previousVmConfig.MemReserve.Bytes !=
                                                 refresh.Server.VMConfig.MemReserve.Bytes;
                                }
                                else
                                {
                                    // If one is null, it's a change. If both are null, it's not.
                                    hasChanged = !(previousVmConfig.MemReserve == null && previousVmConfig.MemReserve == null);
                                }
                            }

                            if (hasChanged)
                            {
                                if (vmThreshold.CriticalThreshold.Enabled)
                                {
                                    state = MonitoredState.Critical;
                                }
                                else if (vmThreshold.WarningThreshold.Enabled)
                                {
                                    state = MonitoredState.Warning;
                                }
                                else
                                {
                                    state = MonitoredState.Informational;
                                }

                                // PR17607 - only one can be active at a time no matter if a different value changed.
                                //objectName.AdditionalQualifiers = new string[]
                                //{
                                //    previousVmConfig.NumCPUs.ToString(),
                                //    previousVmConfig.CPULimit.ToString(),
                                //    previousVmConfig.CPUReserve.ToString(),
                                //    previousVmConfig.MemSize.Megabytes.ToString(),
                                //    previousVmConfig.MemLimit.Megabytes.ToString(),
                                //    previousVmConfig.MemReserve.Megabytes.ToString()
                                //};

                                var additionalData = new[] { previousVmConfig, refresh.Server.VMConfig };
                                CheckEvents((int)Metric.VmConfigChange, objectName, state, true, false, additionalData, null, false);


                                if (metricDeviationMap.ContainsKey(objectName))
                                {
                                    metricDeviationMap.Remove(objectName);
                                }
                            }
                        }
                    }
                }
                // Reraise Alerts that have not expired or if being processed by the SeverPing
                vmAdvancedSettings = GetAdvancedConfiguration(vmThreshold);
                alertDuration = DateTime.UtcNow - GetVMAlertActiveDuration(vmAdvancedSettings);
                foreach (MetricState deviation in metricDeviationMap.Values)
                {
                    if (deviation.LastUpdate >= alertDuration)
                    {
                        if (!deviation.Acknowledged && vmThreshold.IsEnabled)
                        {
                            if (vmThreshold.CriticalThreshold.Enabled)
                            {
                                state = MonitoredState.Critical;
                            }
                            else if (vmThreshold.WarningThreshold.Enabled)
                            {
                                state = MonitoredState.Warning;
                            }
                            else
                            {
                                state = MonitoredState.Informational;
                            }

                            CheckEvents((int)Metric.VmConfigChange, deviation.MonitoredObject, state, true, false, deviation.AdditionalData, null, true);
                        }
                    }
                    else
                    {
                        stateGraph.ClearEvent(deviation);
                    }
                }

                state = MonitoredState.None;

                // VM Host Server Changed
                vmThreshold = null;
                metricDeviationMap = stateGraph.GetEvents(Metric.VmHostServerChange);

                vmThreshold = GetThresholdEntry(Metric.VmHostServerChange);
                if ((vmThreshold != null) && vmThreshold.IsEnabled)
                {
                    if (previousVmConfig != null)
                    {
                        bool hasChanged = false;

                        // If neither is null, compare values
                        if (previousVmConfig.ESXHost != null && refresh.Server.VMConfig.ESXHost != null)
                        {
                            if (previousVmConfig.ESXHost.Name != null && refresh.Server.VMConfig.ESXHost.Name != null && previousVmConfig.ESXHost.Name.Length > 0 && refresh.Server.VMConfig.ESXHost.Name.Length > 0)
                                hasChanged = (previousVmConfig.ESXHost.Name != refresh.Server.VMConfig.ESXHost.Name);
                        }

                        if (hasChanged)
                        {
                            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);

                            if (vmThreshold.CriticalThreshold.Enabled)
                            {
                                state = MonitoredState.Critical;
                            }
                            else if (vmThreshold.WarningThreshold.Enabled)
                            {
                                state = MonitoredState.Warning;
                            }
                            else
                            {
                                state = MonitoredState.Informational;
                            }

                            objectName.AdditionalQualifiers = new string[]
                                                                  {
                                                                      previousVmConfig.ESXHost != null ? previousVmConfig.ESXHost.Name : ""
                                                                  };

                            CheckEvents((int)Metric.VmHostServerChange, objectName, state, true, false, refresh.Server.VMConfig, null, false);

                            if (metricDeviationMap.ContainsKey(objectName))
                            {
                                metricDeviationMap.Remove(objectName);
                            }
                        }
                    }
                }
                vmAdvancedSettings = GetAdvancedConfiguration(vmThreshold);
                alertDuration = DateTime.UtcNow - GetVMAlertActiveDuration(vmAdvancedSettings);
                foreach (MetricState deviation in metricDeviationMap.Values)
                {
                    if (deviation.LastUpdate >= alertDuration)
                    {
                        if (!deviation.Acknowledged && vmThreshold.IsEnabled)
                        {
                            if (vmThreshold.CriticalThreshold.Enabled)
                            {
                                state = MonitoredState.Critical;
                            }
                            else if (vmThreshold.WarningThreshold.Enabled)
                            {
                                state = MonitoredState.Warning;
                            }
                            else
                            {
                                state = MonitoredState.Informational;
                            }

                            CheckEvents((int)Metric.VmHostServerChange, deviation.MonitoredObject, state, true, false, deviation.AdditionalData, null, true);
                        }
                    }
                    else
                    {
                        stateGraph.ClearEvent(deviation);
                    }
                }

                state = MonitoredState.None;

                // VM CPU Utilization
                // TODO: Figure out why vCenter sometimes returns -1
                if (refresh.Server.VMConfig.PerfStats.CpuUsage >= 0)
                {
                    value = valueAdapter.GetVmValue(Metric.VmCPUUtilization, vmThreshold.IsBaselineEnabled);//SQLdm 10.0 (Tarun Sapra)- Get the value from the adapter
                }
                else
                {
                    value = null;
                }

                ProcessMetric((int)Metric.VmCPUUtilization, serverName, value, refresh.Server.VMConfig);

                // VM Memory Utilization
                // TODO: Figure out why vCenter sometimes returns -1
                if (refresh.Server.VMConfig.PerfStats.MemUsage >= 0)
                {
                    value = refresh.Server.VMConfig.PerfStats.MemUsage;
                }
                else
                {
                    value = null;
                }

                ProcessMetric((int)Metric.VmMemoryUtilization, serverName, value, refresh.Server.VMConfig);


                // VM CPU Ready Wait Time
                // TODO: Figure out why vCenter sometimes returns -1
                if (refresh.Server.VMConfig.PerfStats.CpuReady >= 0)
                {
                    value = refresh.Server.VMConfig.PerfStats.CpuReady;
                }
                else
                {
                    value = null;
                }

                if (refresh.Server.VMConfig.ESXHost != null && refresh.Server.VMConfig.PerfStats != null)
                {
                    ProcessMetric((int)Metric.VmCPUReadyWaitTime, serverName, value, refresh.Server.VMConfig);
                }


                // VM Reclaimed/Ballooned Memory
                // TODO: Figure out why vCenter sometimes returns -1
                if ((refresh.Server.VMConfig.PerfStats.MemBallooned != null) &&
                    (refresh.Server.VMConfig.PerfStats.MemBallooned.Kilobytes.HasValue) &&
                    (refresh.Server.VMConfig.PerfStats.MemBallooned.Kilobytes >= 0))
                {
                    value = refresh.Server.VMConfig.PerfStats.MemBallooned.Kilobytes;
                    ProcessMetric((int)Metric.VmReclaimedMemory, serverName, value, refresh.Server.VMConfig);
                }

                // VM Memory Swap Delay Detected
                state = MonitoredState.None;
                vmThreshold = null;
                metricDeviationMap = stateGraph.GetEvents(Metric.VmMemorySwapDelayDetected);

                vmThreshold = GetThresholdEntry(Metric.VmMemorySwapDelayDetected);
                if ((vmThreshold != null) && vmThreshold.IsEnabled)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    bool activelySwapping = (refresh.Server.VMConfig.PerfStats != null && refresh.Server.VMConfig.PerfStats.MemSwapInRate > 0) ||
                                            (refresh.Server.VMConfig.PerfStats != null && refresh.Server.VMConfig.PerfStats.MemSwapOutRate > 0);

                    if (activelySwapping)
                    {
                        if (vmThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (vmThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else
                        {
                            state = MonitoredState.Informational;
                        }
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    CheckEvents((int)Metric.VmMemorySwapDelayDetected, objectName, state, true, false, refresh.Server.VMConfig, null, false);
                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }

                // VM Resource Limits Detected
                state = MonitoredState.None;
                vmThreshold = null;
                metricDeviationMap = stateGraph.GetEvents(Metric.VmResourceLimits);

                vmThreshold = GetThresholdEntry(Metric.VmResourceLimits);
                if ((vmThreshold != null) && vmThreshold.IsEnabled)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    bool resourceLimitsSet = ((refresh.Server.VMConfig.CPULimit > 0) ||
                                              (refresh.Server.VMConfig.MemLimit != null && refresh.Server.VMConfig.MemLimit.Megabytes > 0));

                    if (resourceLimitsSet)
                    {
                        if (vmThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (vmThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else
                        {
                            state = MonitoredState.Informational;
                        }

                        CheckEvents((int)Metric.VmResourceLimits, objectName, state, true, false, refresh.Server.VMConfig, null, false);

                        if (metricDeviationMap.ContainsKey(objectName))
                        {
                            metricDeviationMap.Remove(objectName);
                        }
                    }
                    else
                    {
                        ClearEvents((int)Metric.VmResourceLimits);
                    }
                }

                // VM ESX CPU Utilization
                // TODO: Figure out why vCenter sometimes returns -1
                if (refresh.Server.VMConfig.ESXHost.PerfStats.CpuUsage >= 0)
                {
                    value = refresh.Server.VMConfig.ESXHost.PerfStats.CpuUsage;
                }
                else
                {
                    value = null;
                }

                ProcessMetric((int)Metric.VmESXCPUUtilization, serverName, value, refresh.Server.VMConfig);

                // VM ESX Memory Utilization
                // TODO: Figure out why vCenter sometimes returns -1
                if (refresh.Server.VMConfig.ESXHost.PerfStats.MemUsage >= 0)
                {
                    value = refresh.Server.VMConfig.ESXHost.PerfStats.MemUsage;
                }
                else
                {
                    value = null;
                }

                ProcessMetric((int)Metric.VmESXMemoryUsage, serverName, value, refresh.Server.VMConfig);

                // ESX Memory Swapping Detected
                state = MonitoredState.None;
                vmThreshold = null;
                metricDeviationMap = stateGraph.GetEvents(Metric.VmESXMemorySwapDetected);

                vmThreshold = GetThresholdEntry(Metric.VmESXMemorySwapDetected);
                if ((vmThreshold != null) && vmThreshold.IsEnabled)
                {
                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    bool activelySwapping = (refresh.Server.VMConfig.ESXHost.PerfStats != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemSwapInRate > 0) ||
                                            (refresh.Server.VMConfig.ESXHost.PerfStats != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemSwapOutRate > 0);

                    if (activelySwapping)
                    {
                        if (vmThreshold.CriticalThreshold.Enabled)
                        {
                            state = MonitoredState.Critical;
                        }
                        else if (vmThreshold.WarningThreshold.Enabled)
                        {
                            state = MonitoredState.Warning;
                        }
                        else
                        {
                            state = MonitoredState.Informational;
                        }
                    }
                    else
                    {
                        state = MonitoredState.OK;
                    }

                    CheckEvents((int)Metric.VmESXMemorySwapDetected, objectName, state, true, false, refresh.Server.VMConfig, null, false);

                    if (metricDeviationMap.ContainsKey(objectName))
                    {
                        metricDeviationMap.Remove(objectName);
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error processing virtualization alerts. Continuing with alert processing.", e);
            }
        }

        /// <summary>
        /// Re-raise maintained over time alerts. The Server must be inaccessible in order to evaluate this maintained over time alerts
        /// </summary>
        /// <param name="refresh">The ScheduledRefresh refresh data.</param>
        private void ReRaiseMaintainedOverTimeAlerts(ScheduledRefresh refresh)
        {
            ProcessJobAlerts(refresh);
            ProcessClusterFailover(refresh);
        }

        private void ReRaiseVMAlerts()
        {
            List<Metric> vmMetricList = new List<Metric>();

            vmMetricList.Add(Metric.VmCPUUtilization);
            vmMetricList.Add(Metric.VmConfigChange);
            vmMetricList.Add(Metric.VmESXMemoryUsage);
            vmMetricList.Add(Metric.VmCPUReadyWaitTime);
            vmMetricList.Add(Metric.VmESXCPUUtilization);
            vmMetricList.Add(Metric.VmMemoryUtilization);
            vmMetricList.Add(Metric.VmReclaimedMemory);
            vmMetricList.Add(Metric.VmPowerState);
            vmMetricList.Add(Metric.VmHostServerChange);
            vmMetricList.Add(Metric.VmESXPowerState);
            vmMetricList.Add(Metric.VmESXMemorySwapDetected);
            vmMetricList.Add(Metric.VmMemorySwapDelayDetected);
            vmMetricList.Add(Metric.VmResourceLimits);

            foreach (Metric vmMetric in vmMetricList)
            {
                IDictionary<MonitoredObjectName, MetricState> metricDeviationMap = stateGraph.GetEvents(vmMetric);

                foreach (MetricState deviation in metricDeviationMap.Values)
                {
                    if (!deviation.Acknowledged)
                    {
                        CheckEvents((int)vmMetric, deviation.MonitoredObject, deviation.MonitoredState, (IComparable)deviation.Value,
                                    false, deviation.AdditionalData, null, true);
                    }
                }
            }
        }

        private void ClearDatabaseAlerts(string serverName, string databaseName, string tableName, MonitoredObjectStateGraph stateGraph)
        {
            MonitoredObjectName objectName = new MonitoredObjectName(serverName, databaseName, tableName);

            MetricThresholdEntry thresholdEntry = null;
            AdvancedAlertConfigurationSettings advancedSettings = null;
            IComparable value = null;
            foreach (MetricState deviation in stateGraph.GetEvents(objectName))
            {
                if (thresholdEntry == null || thresholdEntry.MetricID != deviation.MetricID)
                {
                    thresholdEntry = GetThresholdEntry(deviation.MetricID, databaseName);
                    if (thresholdEntry != null)
                    {
                        advancedSettings = GetAdvancedConfiguration(thresholdEntry);
                    }
                }

                // VH - I don't think this is necessary with the Database Alerting split
                //if (refresh.DatabaseSizeError != null)
                //{
                //    stateGraph.ClearEvent(deviation);
                //    continue;
                //}

                if (deviation.MetricID == (int)Metric.DatabaseStatus)
                {
                    value = DatabaseStatus.Undetermined;
                }
                else
                {
                    value = 0;
                }

                CheckEvents(deviation.MetricID, deviation.MonitoredObject, MonitoredState.OK, value, false, deviation.AdditionalData, advancedSettings, false);
            }
        }

        private CheckAction CheckEvents(int metricID, MonitoredObjectName target, MonitoredState newState, IComparable value, bool invalidObject, object additionalData,
                                        AdvancedAlertConfigurationSettings advancedSettings, bool continued, object metricValue = null)
        {
            return CheckEvents(metricID, target, newState, value, invalidObject, additionalData, advancedSettings, continued, false, metricValue);
        }

        private CheckAction CheckEvents(int metricID, MonitoredObjectName target, MonitoredState newState, IComparable value, bool invalidObject, object additionalData,
                                        AdvancedAlertConfigurationSettings advancedSettings, bool continued, bool isBaselineEnabled, object metricValue = null)
        {
            //If metricValue is null give it IComparable value
            if (metricValue == null && metricValue != value)
                metricValue = value;
            CheckAction result = CheckAction.None;
            if (newState == MonitoredState.None)
            {
                return result;
            }

            MetricState deviation = null;
            try
            {
                deviation = stateGraph.GetEvent(target, metricID);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Failed ToString lookup prior deviation for Id={0} newState={1} - skipping event", metricID, newState);
            }

            bool isSmoothed = false;
            bool isFiltered = AlertFilterHelper.IsFiltered(advancedSettings, target, additionalData) == AlertFilterReason.Filtered;

            //SQLDM-30444
            int snapshotFilterThreshold = 0;
            int totalSnapshotCount = 0;
            if (advancedSettings != null)
            {
                snapshotFilterThreshold = advancedSettings.ThresholdTime;
                totalSnapshotCount = advancedSettings.ThresholdTime;
            }
            bool isAlertable = false;

            if (deviation != null)
            {
                //SQLDM-30444
				//SQLDM-30825
                if (!isFiltered)
                {
                    isAlertable = AlertFilterHelper.IsAlertable(deviation, snapshotFilterThreshold, totalSnapshotCount);
                    isFiltered = !isAlertable;
                }

                if (!isFiltered)
                {
                    // SQLDM -19783 (Varun Chopra) Handle states for Alert Suppression
                    isSmoothed = AlertFilterHelper.IsSmoothed(advancedSettings, timestamp, deviation.LastUpdate);
                    isFiltered = isSmoothed;
                }

                //  deviation is cleared
                if (newState == MonitoredState.OK)
                {
                    // raise alert if not filtered or was already sent to ms
                    if (!isFiltered || deviation.NeedsClearing)
                    {
                        // update the last update time 
                        deviation.LastUpdate = timestamp;

                        // Set the Needs Clearing to true
                        deviation.NeedsClearing = true;

                        // don't add event if the alert was acknowledged don't add event if previous state was ok
                        // SQLDM -19783 (Varun Chopra) Handle states for Alert Suppression
                        if (!deviation.Acknowledged && deviation.MonitoredState != MonitoredState.OK)
                        {
                            deviation.Value = value;
                            deviation.MetricValue = metricValue;
                            deviation.IsBaselineEnabled = isBaselineEnabled;
                            deviation.AdditionalData = additionalData;
                            // SQLDM -19783 (Varun Chopra) Handle states for Alert Suppression update the correct state
                            deviation.MonitoredState = newState;
                            alertableRefresh.AddEvent(new StateDeviationClearEvent(deviation));
                        }
                    }

                    stateGraph.ClearEvent(deviation);
                    result = CheckAction.Clear;
                }
                else
                {
                    // deviation did not clear (state or value may have changed)

                    // SQLDM -19783 (Varun Chopra) Handle states for Alert Suppression - Allow deviation to be processed post alert suppression interval
                    if (!isFiltered && !deviation.Acknowledged)
                    {
                        //SQLDM-30139.
                        bool stateChanged = deviation.MonitoredState != newState;

                        if (!(isSmoothed && !stateChanged))
                        {
                            // if continued (we are propagating events for items that don't auto clear)                
                            switch (metricID)
                            {
                                case (int)Metric.BombedJobs:
                                case (int)Metric.LongJobs:
                                case (int)Metric.LongJobsMinutes:
                                case (int)Metric.JobCompletion:
                                case (int)Metric.ReorganisationPct:
                                case (int)Metric.ClusterFailover:
                                case (int)Metric.VmConfigChange:
                                case (int)Metric.VmHostServerChange:
                                    if (!continued)
                                    {
                                        deviation.StartTime = deviation.LastUpdate = timestamp;
                                    }

                                    break;
                                default:
                                    deviation.LastUpdate = timestamp;
                                    break;
                            }
                        }

                        MonitoredState previousState = deviation.MonitoredState;

                        deviation.AdditionalData = additionalData;

                        if (stateChanged)
                        {
                            deviation.MonitoredState = newState;

                            //SQLDM-3044-- Resetting the count after the state is changed
                            deviation.TimesThresholdCrossed = 0;
                        }
                        else //SQLDM-30444
                        {
                            deviation.TimesThresholdCrossed += 1;
                            
                        }

                        if(deviation.TotalThresholdLimit == totalSnapshotCount)
                        {
                            deviation.TotalThresholdLimit = 0; //Resetting the limit after the threshold is reached.
                        }
                        else
                        {
                            deviation.TotalThresholdLimit++;
                        }
                        

                        if (!value.Equals(deviation.Value))
                        {
                            deviation.Value = value;
                        }
                        if (!metricValue.Equals(deviation.MetricValue))
                        {
                            deviation.MetricValue = metricValue;
                        }
                        deviation.IsBaselineEnabled = isBaselineEnabled;
                        bool wasSmoothed = deviation.Smoothed;

                        

                        // if previously smoothed and we never sent event to ms then issue is new
                        if (wasSmoothed && !deviation.NeedsClearing)
                        {
                            alertableRefresh.AddEvent(deviation.CachedEvent);
                        }
                        else
                        {
                            // issue previously sent to ms so sent an update event
                            StateDeviationUpdateEvent sdue = continued
                                                                 ? new StateDeviationUpdateEvent(deviation.CachedEvent, timestamp)
                                                                 : new StateDeviationUpdateEvent(deviation);

                            sdue.StateChanged = stateChanged;
                            sdue.PreviousState = previousState;
                            alertableRefresh.AddEvent(sdue);
                        }
                        if (!deviation.NeedsClearing)
                        {
                            deviation.NeedsClearing = true;
                        }
                    }
                    //SQLDM-30139.
                    // update internal state with new smoothing state
                    if(!(!(deviation.MonitoredState != newState) && isSmoothed))
                    {
                        deviation.Smoothed = isSmoothed;
                    }
                    result = CheckAction.Update;
                }
            }
            else
            {
                if (newState > MonitoredState.OK) //New deviation
                {
                    deviation = new MetricState(target, metricID, timestamp);
                    deviation.Value = value;
                    deviation.MetricValue = metricValue;
                    deviation.IsBaselineEnabled = isBaselineEnabled;
                    deviation.MonitoredState = newState;
                    deviation.AdditionalData = additionalData;

                    // set flag if smoothing enabled for this alert
                    deviation.Smoothed = AlertFilterHelper.IsSmoothingEnabled(advancedSettings);

                    //SQLDM-30444
					//SQLDM-30825
					if (!isFiltered)
					{
						isAlertable = AlertFilterHelper.IsAlertable(deviation, snapshotFilterThreshold, totalSnapshotCount);
						isFiltered = !isAlertable;
					}
                    // if it is smoothed we don't need to add the event
                    if (!isFiltered)
                    {
                        isFiltered = deviation.Smoothed;
                    }

                    // if we are sending to the ms then we may need to clear it in the future
                    deviation.NeedsClearing = !isFiltered;

                    // keep track of the issue
                    stateGraph.AddEvent(deviation);

                    // send the new event to the management service
                    if (!isFiltered)
                    {
                        alertableRefresh.AddEvent(deviation.CachedEvent);
                    }

                    result = CheckAction.Add;
                }
            }

            return result;
        }

        private void ProcessAlwaysOnMetrics(ScheduledRefresh refresh)
        {
            LOG.Debug("Entering ProcessAlwaysOnMetrics");

            // if no longer enabled make sure events get cleared
            if (!IsThresholdEnabled(Metric.AlwaysOnAvailabilityGroupRoleChange))
            {
                ClearEvents(Metric.AlwaysOnAvailabilityGroupRoleChange);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnEstimatedDataLossTime))
            {
                ClearEvents(Metric.AlwaysOnEstimatedDataLossTime);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnSynchronizationHealthState))
            {
                ClearEvents(Metric.AlwaysOnSynchronizationHealthState);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnEstimatedRecoveryTime))
            {
                ClearEvents(Metric.AlwaysOnEstimatedRecoveryTime);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnSynchronizationPerformance))
            {
                ClearEvents(Metric.AlwaysOnSynchronizationPerformance);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnLogSendQueueSize))
            {
                ClearEvents(Metric.AlwaysOnLogSendQueueSize);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnRedoQueueSize))
            {
                ClearEvents(Metric.AlwaysOnRedoQueueSize);
            }
            if (!IsThresholdEnabled(Metric.AlwaysOnRedoRate))
            {
                ClearEvents(Metric.AlwaysOnRedoRate);
            }

            LOG.Debug("ProcessMirroringMetrics - Cleared events");

            MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
            MonitoredState state = 0;
            IComparable value = null;

            //don;t continue if there are no Availability Groups details in this refresh
            if (refresh.AvailabilityGroupsSnapshot == null)
            {
                return;
            }

            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnEstimatedDataLossTime = stateGraph.GetEvents(Metric.AlwaysOnEstimatedDataLossTime);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnSynchronizationHealthState = stateGraph.GetEvents(Metric.AlwaysOnSynchronizationHealthState);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnEstimatedRecoveryTime = stateGraph.GetEvents(Metric.AlwaysOnEstimatedRecoveryTime);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnSynchronizationPerformance = stateGraph.GetEvents(Metric.AlwaysOnSynchronizationPerformance);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnLogSendQueueSize = stateGraph.GetEvents(Metric.AlwaysOnLogSendQueueSize);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnRedoQueueSize = stateGraph.GetEvents(Metric.AlwaysOnRedoQueueSize);
            IDictionary<MonitoredObjectName, MetricState> deviationMapAlwaysOnRedoRate = stateGraph.GetEvents(Metric.AlwaysOnRedoRate);

            /////////Process the AlwaysOnAvailabilityGroupRoleChange Alert ////////
            ProcessAlwaysOnAvailabilityGroupRoleChange(refresh);

            foreach (AlwaysOnStatistics alwaysOnStatistics in refresh.AvailabilityGroupsSnapshot.ReplicaStatistics)
            {
                ProcessAlwaysOnEstimatedDataLossTime(refresh, alwaysOnStatistics, deviationMapAlwaysOnEstimatedDataLossTime);
                ProcessAlwaysOnSynchronizationHealthState(refresh, alwaysOnStatistics, deviationMapAlwaysOnSynchronizationHealthState);
                ProcessAlwaysOnEstimatedRecoveryTime(refresh, alwaysOnStatistics, deviationMapAlwaysOnEstimatedRecoveryTime);
                ProcessAlwaysOnSynchronizationPerformance(refresh, alwaysOnStatistics, deviationMapAlwaysOnSynchronizationPerformance);
                ProcessAlwaysOnLogSendQueueSize(refresh, alwaysOnStatistics, deviationMapAlwaysOnLogSendQueueSize);
                ProcessAlwaysOnRedoQueueSize(refresh, alwaysOnStatistics, deviationMapAlwaysOnRedoQueueSize);
                ProcessAlwaysOnRedoRate(refresh, alwaysOnStatistics, deviationMapAlwaysOnRedoRate);
            }

            if (deviationMapAlwaysOnEstimatedDataLossTime != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnEstimatedDataLossTime.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnEstimatedDataLossTime, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnSynchronizationHealthState != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnSynchronizationHealthState.Values)
                {
                    //Setting the Health status to healthy when the alert state is moved to OK
                    object[] additionalDataNew = deviation.AdditionalData as object[];
                    additionalDataNew[1] = "";
                    deviation.AdditionalData = additionalDataNew;

                    CheckEvents((int)Metric.AlwaysOnSynchronizationHealthState, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnEstimatedRecoveryTime != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnEstimatedRecoveryTime.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnEstimatedRecoveryTime, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnSynchronizationPerformance != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnSynchronizationPerformance.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnSynchronizationPerformance, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnLogSendQueueSize != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnLogSendQueueSize.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnLogSendQueueSize, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnRedoQueueSize != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnRedoQueueSize.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnRedoQueueSize, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

            if (deviationMapAlwaysOnRedoRate != null)
            {
                foreach (MetricState deviation in deviationMapAlwaysOnRedoRate.Values)
                {
                    CheckEvents((int)Metric.AlwaysOnRedoRate, deviation.MonitoredObject, MonitoredState.OK, 0, false, deviation.AdditionalData, null, false);
                }
            }

        }



        private void ProcessAlwaysOnAvailabilityGroupRoleChange(ScheduledRefresh refresh)
        {
            LOG.Debug(string.Format("AlwaysOn: Getting previous refresh for {0}", serverName.ServerName));
            LOG.Debug(string.Format("AlwaysOn: workload.PreviousRefresh {0} null", workload.PreviousRefresh == null ? "is" : "is not"));

            if (workload.PreviousRefresh != null)
            {
                MetricThresholdEntry alwaysOnAvailabilityGroupRoleChangeThreshold = GetThresholdEntry(Metric.AlwaysOnAvailabilityGroupRoleChange);
                if (alwaysOnAvailabilityGroupRoleChangeThreshold != null && alwaysOnAvailabilityGroupRoleChangeThreshold.IsEnabled)
                {
                    LOG.Debug(string.Format("AlwaysOn: AlwaysOnAvailabilityGroupRoleChange Alert is enabled and we have a previous refresh."));
                    IDictionary<MonitoredObjectName, MetricState> deviationMap = stateGraph.GetEvents(Metric.AlwaysOnAvailabilityGroupRoleChange);
                    LOG.Debug(string.Format("AlwaysOn: Got deviation map for session role change."));

                    MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName);
                    MonitoredState state = 0;
                    IComparable value = null;

                    //Get the last refresh to see if the role has changed
                    ScheduledRefresh lastRefresh = (ScheduledRefresh)workload.PreviousRefresh;
                    LOG.Debug(string.Format("AlwaysOn: Previous refresh was at {0}", lastRefresh.TimeStamp));

                    // SQLDM-26298 - Code changes to the availability group role change
                    AdvancedAlertConfigurationSettings alwaysOnAvailabilityGroupRoleChangeAdvanced = GetAdvancedConfiguration(alwaysOnAvailabilityGroupRoleChangeThreshold);
                    LOG.Debug(string.Format("Got role change advanced settings {0}", alwaysOnAvailabilityGroupRoleChangeAdvanced == null ? "but it is null" : "and it is valid"));

                    List<Guid> replicaEvaluatedForRoleChange = new List<Guid>();
                    IDictionary<Guid, AvailabilityGroup> availabilityGroups = refresh.AvailabilityGroupsSnapshot.AvailabilityGroups;
                    if (availabilityGroups != null && availabilityGroups.Count > 0)
                    {
                        foreach (AvailabilityGroup avlGroups in availabilityGroups.Values)
                        {
                            LOG.Debug(string.Format("AlwaysOn: GroupName [{0}], Replica Counts [{1}]", avlGroups.GroupName, avlGroups.Replicas.Count));
                            IDictionary<Guid, AvailabilityReplica> replicas = avlGroups.Replicas;
                            if (replicas != null && replicas.Count > 0)
                            {
                                foreach (AvailabilityReplica replica in replicas.Values)
                                {
                                    // SQLDM-26298 - Code changes to the availability group role change
                                    string databaseAffected = "";

                                    LOG.Debug(string.Format("AlwaysOn: Role Changed for Replica = \"{0}\"", replica.ReplicaName));
                                    foreach (AlwaysOnDatabase db in replica.DatabaseItems)
                                    {
                                        if (databaseAffected.Length > 0)
                                            databaseAffected += ", ";
                                        databaseAffected += db.DatabaseName;
                                    }

                                    LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                                    object[] additionalData = new object[]
                                          {
                                                    replica.ReplicaName,
                                                    replica.ReplicaRole.ToString(),
                                                    replica.ReplicaRole.ToString(),
                                                    databaseAffected
                                          };

                                    state = MonitoredState.OK;
                                    value = state;

                                    //Check if the Role change checking is already done for this replica if yes then skip
                                    LOG.Debug(string.Format("AlwaysOn: Checking replica id [{0}], Replica Name [{1}]", replica.ReplicaId, replica.ReplicaName));
                                    if (replicaEvaluatedForRoleChange.Contains(replica.ReplicaId))
                                        continue;

                                    LOG.Debug(string.Format("AlwaysOn: Adding to evaluated replica id [{0}]", replica.ReplicaId));
                                    replicaEvaluatedForRoleChange.Add(replica.ReplicaId);

                                    // SQLDM-26298 - Code changes to get Number of Minutes for the alert can be active
                                    int intMinutesAgeAlerts = 0;
                                    if (alwaysOnAvailabilityGroupRoleChangeAdvanced != null)
                                    {
                                        intMinutesAgeAlerts = alwaysOnAvailabilityGroupRoleChangeAdvanced.NumberMinutesAgingAlertsAreActive;
                                    }
                                    else
                                    {
                                        intMinutesAgeAlerts = 720;
                                        LOG.Debug(String.Format("There is no advanced role setting so we are going with a default age of {0}", intMinutesAgeAlerts));
                                    }

                                    // SQLDM-26298 - Update the Alerts table to set the Active = 0 if there are any critical/warning/informational recrods for more than intMinutesAgeAlerts time
                                    try
                                    {
                                        IManagementService mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                        mgmtSvc.UpdateAGAlertLogRecord(DateTime.UtcNow, objectName.ToString(), (int)Metric.AlwaysOnAvailabilityGroupRoleChange, intMinutesAgeAlerts);

                                    }
                                    catch (Exception e)
                                    {
                                        LOG.Warn(string.Format("Alwayson: Error while updateing the Availability group role related entries in the Metric Table",
                                            avlGroups.GroupId, replica.ReplicaId));
                                        if (!(e is KeyNotFoundException))
                                            LOG.Error(e);
                                    }

                                    try
                                    {
                                        AvailabilityReplica previousReplica = lastRefresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(avlGroups.GroupId, replica.ReplicaId);

                                        //in the first instance there wont be any previous replica
                                        if (previousReplica == null)
                                            previousReplica = replica;

                                        bool blnRoleHasChanged = replica.ReplicaRole != previousReplica.ReplicaRole;
                                        bool blnsetAlert = false;

                                        if (blnRoleHasChanged)
                                        {
                                            blnsetAlert = true;
                                        }
                                        if (deviationMap != null && deviationMap.Count > 0 && deviationMap.ContainsKey(objectName))
                                        {

                                            additionalData = new object[]
                                            {
                                                    replica.ReplicaName,
                                                    previousReplica.ReplicaRole.ToString(),
                                                    replica.ReplicaRole.ToString(),
                                                    databaseAffected
                                            };

                                            //if we have past the reqired aging period

                                            var deviation = deviationMap[objectName];

                                            //if the alert is within the aging period
                                            // SQLDM-26298 - Compare the condition with local current date time
                                            if (deviation.StartTime.AddMinutes(intMinutesAgeAlerts).ToLocalTime() > DateTime.Now)
                                            {
                                                LOG.Debug(string.Format("We have passed the aging period"));

                                                //send the ok event
                                                state = MonitoredState.OK;
                                                //Add a clear here if you want to clear the aged role change alert without it being lowered with a green tick
                                            }
                                            else
                                            {
                                                LOG.Debug(string.Format("Alert must clear at {0}", deviation.StartTime.AddMinutes(intMinutesAgeAlerts).ToLocalTime()));

                                                //We are still in the aging period. If the role has changed again we need to raise a new alert
                                                if (blnRoleHasChanged)
                                                {
                                                    LOG.Debug(string.Format("The role has changed again so we must clear the old alert and raise a new one."));
                                                    //remove the deviation from the deviationmap to the alert 
                                                    //shows up as a new one rather than a continuation of the old one
                                                    if (deviationMap.ContainsKey(objectName))
                                                    {
                                                        deviationMap.Remove(objectName);
                                                    }
                                                    //clear this event from the stategraph
                                                    stateGraph.ClearEvent(deviation);
                                                }

                                                // SQLDM-26298 - Fire the OK alert as the alert time gone pass the intMinutesAgeAlerts duration
                                                state = MonitoredState.OK;
                                                value = state;
                                                CheckEvents((int)Metric.AlwaysOnAvailabilityGroupRoleChange, objectName, state, value, false, additionalData,
                                                            null, false);
                                                LOG.Debug(string.Format("AlwaysOn: Checkevents for Always On Group Availability role change has been performed."));
                                            }
                                        }
                                        else //There is no existing deviation
                                        {
                                            LOG.Debug(string.Format("This role change alert is being raised for the first time (or at least it is not already active)"));
                                        }

                                        // Role changed - Fail Over Happened
                                        if (blnsetAlert)
                                        {
                                            additionalData = new object[]
                                          {
                                                    replica.ReplicaName,
                                                    previousReplica.ReplicaRole.ToString(),
                                                    replica.ReplicaRole.ToString(),
                                                    databaseAffected
                                          };

                                            LOG.Debug(string.Format("AlwaysOn: Raising the alert [{0}], [{1}], [{2}], [{3}],",
                                                replica.ReplicaName, previousReplica.ReplicaRole.ToString(),
                                                replica.ReplicaRole.ToString(), databaseAffected));

                                            //set the state according to the alert thresholds
                                            if (alwaysOnAvailabilityGroupRoleChangeThreshold.CriticalThreshold.Enabled)
                                            {
                                                state = MonitoredState.Critical;
                                            }
                                            else if (alwaysOnAvailabilityGroupRoleChangeThreshold.WarningThreshold.Enabled)
                                            {
                                                state = MonitoredState.Warning;
                                            }
                                            else if (alwaysOnAvailabilityGroupRoleChangeThreshold.InfoThreshold.Enabled)
                                            {
                                                state = MonitoredState.Informational;
                                            }
                                            else
                                            {
                                                state = MonitoredState.OK;
                                            }

                                            value = state;
                                            CheckEvents((int)Metric.AlwaysOnAvailabilityGroupRoleChange, objectName, state, value, false, additionalData,
                                                        null, false);
                                            LOG.Debug(string.Format("AlwaysOn: Checkevents for Always On Group Availability role change has been performed."));

                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        LOG.Warn(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                                            avlGroups.GroupId, replica.ReplicaId));
                                        if (!(e is KeyNotFoundException))
                                            LOG.Error(e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string GetAffectedDatabase(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics)
        {
            string databaseAffected = "";

            try
            {
                AvailabilityReplica replica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId, alwaysOnStatistics.ReplicaId);
                foreach (AlwaysOnDatabase db in replica.DatabaseItems)
                {
                    if (alwaysOnStatistics.DatabaseId == db.DatabaseId)
                    {
                        databaseAffected = db.DatabaseName;
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                LOG.Warn(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                    alwaysOnStatistics.GroupId, alwaysOnStatistics.ReplicaId));
                if (!(e is KeyNotFoundException))
                    LOG.Error(e);
            }

            return databaseAffected;
        }

        private void ProcessAlwaysOnEstimatedDataLossTime(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnEstimatedDataLossTimeThreshold = GetThresholdEntry(Metric.AlwaysOnEstimatedDataLossTime);
            if (alwaysOnEstimatedDataLossTimeThreshold != null && alwaysOnEstimatedDataLossTimeThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnEstimatedDataLossTime Alert is enabled"));
                string databaseAffected = GetAffectedDatabase(refresh, alwaysOnStatistics);
                LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                AvailabilityReplica currentReplica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, databaseAffected);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };
                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnEstimatedDataLossTimeThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnEstimatedDataLossTime);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnEstimatedDataLossTime, out meanValue))
                    {
                        alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean = meanValue;

                        value = alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeAsBaselinePerc =
                                    (alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.EstimatedDataLossTime
                                    / (alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean == 0 ? 1.0 : alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnEstimatedDataLossTime);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.EstimatedDataLossTime;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnEstimatedDataLossTimeThreshold);

                object[] additionalData = new object[] {
                                                        (int)alwaysOnStatistics.EstimatedDataLossTime,
                                                        databaseAffected
                                                  };
                CheckEvents((int)Metric.AlwaysOnEstimatedDataLossTime, objectName, state, value, false, additionalData, null, false, alwaysOnEstimatedDataLossTimeThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.EstimatedDataLossTime);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }

        }

        private void ProcessAlwaysOnSynchronizationHealthState(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnSynchronizationHealthStateThreshold = GetThresholdEntry(Metric.AlwaysOnSynchronizationHealthState);
            AdvancedAlertConfigurationSettings advancedSettings = null;
            if (alwaysOnSynchronizationHealthStateThreshold != null && alwaysOnSynchronizationHealthStateThreshold.IsEnabled)
            {
                advancedSettings = GetAdvancedConfiguration(alwaysOnSynchronizationHealthStateThreshold);
                LOG.Debug(string.Format("AlwaysOnSynchronizationHealthState Alert is enabled"));
                //If the state of the database is Healthy then return
                if (alwaysOnStatistics.SynchronizationDatabaseHealth == AlwaysOnSynchronizationHealth.Healthy)
                    return;

                AlwaysOnDatabase database = refresh.AvailabilityGroupsSnapshot.GetDatabase(alwaysOnStatistics.GroupId,
                                                                                            alwaysOnStatistics.ReplicaId,
                                                                                            alwaysOnStatistics.DatabaseId);

                AvailabilityReplica replica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);
                //Replica Name is added to make the Monitored object unique for Alerting. Since for the given server we can have same
                // database name for different Replica.
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, database.DatabaseName, replica.ReplicaName);
                MonitoredState state = 0;
                IComparable value = null;

                //set the state according to the alert thresholds
                if (alwaysOnSynchronizationHealthStateThreshold.CriticalThreshold.Enabled)
                {
                    state = MonitoredState.Critical;
                }
                else if (alwaysOnSynchronizationHealthStateThreshold.WarningThreshold.Enabled)
                {
                    state = MonitoredState.Warning;
                }
                else if (alwaysOnSynchronizationHealthStateThreshold.InfoThreshold.Enabled)
                {
                    state = MonitoredState.Informational;
                }
                else
                {
                    state = MonitoredState.OK;
                }

                value = state;

                AvailabilityGroup availabilityGroup = null;
                try
                {
                    availabilityGroup = refresh.AvailabilityGroupsSnapshot.GetAvailabilityGroup(alwaysOnStatistics.GroupId);
                }
                catch (Exception e)
                {
                    LOG.Error(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                        alwaysOnStatistics.GroupId, alwaysOnStatistics.ReplicaId));
                    if (!(e is KeyNotFoundException))
                        LOG.Error(e);
                    return;
                }
                string healthStatus = "";
                if (alwaysOnStatistics.SynchronizationDatabaseHealth == AlwaysOnSynchronizationHealth.PartiallyHealthy)
                    healthStatus = "Partially";
                else if (alwaysOnStatistics.SynchronizationDatabaseHealth == AlwaysOnSynchronizationHealth.NotHealthy)
                    healthStatus = "Not";

                object[] additionalData = new object[] {
                                                        availabilityGroup.GroupName,
                                                        healthStatus,
                                                        database.DatabaseName,
                                                        replica.ReplicaName
                                                       };

			//SQLDM-19380. (10.4.1 - Sprint 1) Support Advanced Settings (Alert suppression)for Availability Group Synchronization Health metric
                if (alwaysOnSynchronizationHealthStateThreshold != null )
                {
                    advancedSettings = GetAdvancedConfiguration(alwaysOnSynchronizationHealthStateThreshold);
                }
                CheckEvents((int)Metric.AlwaysOnSynchronizationHealthState, objectName, state, value, false, additionalData, advancedSettings, false);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private void ProcessAlwaysOnEstimatedRecoveryTime(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnEstimatedRecoveryTimeThreshold = GetThresholdEntry(Metric.AlwaysOnEstimatedRecoveryTime);
            if (alwaysOnEstimatedRecoveryTimeThreshold != null && alwaysOnEstimatedRecoveryTimeThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnEstimatedRecoveryTime Alert is enabled"));
                string databaseAffected = GetAffectedDatabase(refresh, alwaysOnStatistics);
                LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                AvailabilityReplica currentReplica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, databaseAffected);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };

                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnEstimatedRecoveryTimeThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnEstimatedRecoveryTime);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnEstimatedRecoveryTime, out meanValue))
                    {
                        alwaysOnStatistics.AlwaysOnEstimatedRecoveryTimeBaselineMean = meanValue;

                        value = alwaysOnStatistics.AlwaysOnEstimatedRecoveryTimeAsBaselinePerc =
                                    (alwaysOnStatistics.AlwaysOnEstimatedRecoveryTimeBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.EstimatedRecoveryTime
                                    / (alwaysOnStatistics.AlwaysOnEstimatedRecoveryTimeBaselineMean == 0 ? 1.0 : alwaysOnStatistics.AlwaysOnEstimatedRecoveryTimeBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnEstimatedRecoveryTime);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.EstimatedRecoveryTime;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnEstimatedRecoveryTimeThreshold);
                object[] additionalData = new object[] {
                                                        (int)alwaysOnStatistics.EstimatedRecoveryTime,
                                                        databaseAffected
                                                  };
                CheckEvents((int)Metric.AlwaysOnEstimatedRecoveryTime, objectName, state, value, false, additionalData, null, false, alwaysOnEstimatedRecoveryTimeThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.EstimatedRecoveryTime);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private void ProcessAlwaysOnSynchronizationPerformance(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnSynchronizationPerformanceThreshold = GetThresholdEntry(Metric.AlwaysOnSynchronizationPerformance);
            if (alwaysOnSynchronizationPerformanceThreshold != null && alwaysOnSynchronizationPerformanceThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnSynchronizationPerformance Alert is enabled"));
                AlwaysOnDatabase database = refresh.AvailabilityGroupsSnapshot.GetDatabase(alwaysOnStatistics.GroupId,
                                                                                            alwaysOnStatistics.ReplicaId,
                                                                                            alwaysOnStatistics.DatabaseId);

                AvailabilityReplica currentReplica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, database.DatabaseName);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };
                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnSynchronizationPerformanceThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnSynchronizationPerformance);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnSynchronizationPerformance, out meanValue))
                    {
                        alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean = meanValue;

                        value = alwaysOnStatistics.SynchronizationPerformanceAsBaselinePerc =
                                    (alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.SynchronizationPerformace
                                    / (alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean == 0 ? 1.0 : alwaysOnStatistics.AlwaysOnEstimatedDataLossTimeBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnSynchronizationPerformance);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.SynchronizationPerformace;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnSynchronizationPerformanceThreshold);
                object[] additionalData = new object[] {
                                                        (int)alwaysOnStatistics.SynchronizationPerformace,
                                                        database.DatabaseName,
                                                        currentReplica.ReplicaName
                                                  };
                CheckEvents((int)Metric.AlwaysOnSynchronizationPerformance, objectName, state, value, false, additionalData, null, false, alwaysOnSynchronizationPerformanceThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.SynchronizationPerformace);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private void ProcessAlwaysOnLogSendQueueSize(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnLogSendQueueSizeThreshold = GetThresholdEntry(Metric.AlwaysOnLogSendQueueSize);
            if (alwaysOnLogSendQueueSizeThreshold != null && alwaysOnLogSendQueueSizeThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnLogSendQueueSize Alert is enabled"));
                string databaseAffected = GetAffectedDatabase(refresh, alwaysOnStatistics);
                LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                AvailabilityReplica currentReplica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);
                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, databaseAffected);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };

                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnLogSendQueueSizeThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnLogSendQueueSize);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnLogSendQueueSize, out meanValue))
                    {
                        alwaysOnStatistics.LogSendQueueSizeBaselineMean = meanValue;

                        value = alwaysOnStatistics.LogSendQueueSizeAsBaselinePerc =
                                    (alwaysOnStatistics.LogSendQueueSizeBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.LogSendQueueSize
                                    / (alwaysOnStatistics.LogSendQueueSizeBaselineMean == 0 ? 1.0 : alwaysOnStatistics.LogSendQueueSizeBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnLogSendQueueSize);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.LogSendQueueSize;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnLogSendQueueSizeThreshold);
                AvailabilityGroup availabilityGroup = null;
                try
                {
                    availabilityGroup = refresh.AvailabilityGroupsSnapshot.GetAvailabilityGroup(alwaysOnStatistics.GroupId);
                }
                catch (Exception e)
                {
                    LOG.Error(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                        alwaysOnStatistics.GroupId, alwaysOnStatistics.ReplicaId));
                    if (!(e is KeyNotFoundException))
                        LOG.Error(e);
                    return;
                }

                object[] additionalData = new object[] {
                                                        currentReplica.ReplicaName,
                                                        (int)alwaysOnStatistics.LogSendQueueSize,
                                                        databaseAffected
                                                       };

                CheckEvents((int)Metric.AlwaysOnLogSendQueueSize, objectName, state, value, false, additionalData, null, false, alwaysOnLogSendQueueSizeThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.LogSendQueueSize);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private void ProcessAlwaysOnRedoQueueSize(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnRedoQueueSizeThreshold = GetThresholdEntry(Metric.AlwaysOnRedoQueueSize);
            if (alwaysOnRedoQueueSizeThreshold != null && alwaysOnRedoQueueSizeThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnRedoQueueSize Alert is enabled"));
                string databaseAffected = GetAffectedDatabase(refresh, alwaysOnStatistics);
                LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                AvailabilityReplica replica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);

                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, databaseAffected);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };

                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnRedoQueueSizeThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnRedoQueueSize);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnRedoQueueSize, out meanValue))
                    {
                        alwaysOnStatistics.RedoQueueSizeBaselineMean = meanValue;

                        value = alwaysOnStatistics.RedoQueueSizeAsBaselinePerc =
                                    (alwaysOnStatistics.RedoQueueSizeBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.RedoQueueSize
                                    / (alwaysOnStatistics.RedoQueueSizeBaselineMean == 0 ? 1.0 : alwaysOnStatistics.RedoQueueSizeBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnRedoQueueSize);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.RedoQueueSize;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnRedoQueueSizeThreshold);
                object[] additionalData = new object[] {
                                                        replica.ReplicaName,
                                                        (int)alwaysOnStatistics.RedoQueueSize,
                                                        databaseAffected
                                                       };

                CheckEvents((int)Metric.AlwaysOnRedoQueueSize, objectName, state, value, false, additionalData, null, false, alwaysOnRedoQueueSizeThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.RedoQueueSize);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private void ProcessAlwaysOnRedoRate(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics, IDictionary<MonitoredObjectName, MetricState> deviationMap)
        {
            MetricThresholdEntry alwaysOnRedoRateThreshold = GetThresholdEntry(Metric.AlwaysOnRedoRate);
            if (alwaysOnRedoRateThreshold != null && alwaysOnRedoRateThreshold.IsEnabled)
            {
                LOG.Debug(string.Format("AlwaysOnRedoRate Alert is enabled"));

                string databaseAffected = GetAffectedDatabase(refresh, alwaysOnStatistics);
                LOG.Debug(string.Format("AlwaysOn: Databases Affected = \"{0}\"", databaseAffected));

                AvailabilityReplica replica = refresh.AvailabilityGroupsSnapshot.GetAvailabilityReplica(alwaysOnStatistics.GroupId,
                                                                                                        alwaysOnStatistics.ReplicaId);

                MonitoredObjectName objectName = new MonitoredObjectName(serverName.ServerName, databaseAffected);
                objectName.AdditionalQualifiers = new string[]
                                                          {
                                                              alwaysOnStatistics.GroupId.ToString(),
                                                              alwaysOnStatistics.ReplicaId.ToString(),
                                                              alwaysOnStatistics.DatabaseId.ToString()
                                                          };
                if ((int)alwaysOnStatistics.RedoRate <= 0)
                    return;

                //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                IComparable value = null;
                if (alwaysOnRedoRateThreshold.IsBaselineEnabled)
                {
                    LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)Metric.AlwaysOnRedoRate);
                    double? meanValue;
                    if (workload.BaselineMetricMeanCollection.GetBaselineMeanScheduled(refresh, (int)Metric.AlwaysOnRedoRate, out meanValue))
                    {
                        alwaysOnStatistics.RedoRateBaselineMean = meanValue;

                        value = alwaysOnStatistics.RedoRateAsBaselinePerc =
                                    (alwaysOnStatistics.RedoRateBaselineMean == null) ? null :
                                    ((int)alwaysOnStatistics.RedoRate
                                    / (alwaysOnStatistics.RedoRateBaselineMean == 0 ? 1.0 : alwaysOnStatistics.RedoRateBaselineMean)) * 100;
                    }
                    else
                    {
                        LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)Metric.AlwaysOnRedoRate);
                    }
                }
                else
                {
                    value = (int)alwaysOnStatistics.RedoRate;
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                MonitoredState state = ScheduledRefreshState.GetState(value, alwaysOnRedoRateThreshold);
                object[] additionalData = new object[] {
                                                        replica.ReplicaName,
                                                        (int)alwaysOnStatistics.RedoRate,
                                                        databaseAffected
                                                       };

                CheckEvents((int)Metric.AlwaysOnRedoRate, objectName, state, value, false, additionalData, null, false, alwaysOnRedoRateThreshold.IsBaselineEnabled, (int)alwaysOnStatistics.RedoRate);

                if (deviationMap != null && deviationMap.ContainsKey(objectName))
                {
                    deviationMap.Remove(objectName);
                }
            }
        }

        private object[] AlwaysOnGetAdditionalAlertInfo(ScheduledRefresh refresh, AlwaysOnStatistics alwaysOnStatistics)
        {
            object[] result = new object[] {
                                ""
                              };

            AlwaysOnDatabase database = refresh.AvailabilityGroupsSnapshot.GetDatabase(alwaysOnStatistics.GroupId,
                                                                                        alwaysOnStatistics.ReplicaId,
                                                                                        alwaysOnStatistics.DatabaseId);



            return result;
        }


        #region interface implementations

        #endregion

        #region nested types

        protected enum CheckAction
        {
            Add,
            Update,
            Clear,
            None
        }

        #endregion
    }


}
