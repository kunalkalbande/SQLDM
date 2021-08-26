//------------------------------------------------------------------------------
// <copyright file="NotificationManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Linq;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.ManagementService.Auditing;
using Idera.SQLdm.ManagementService.Auditing.Actions;

namespace Idera.SQLdm.ManagementService.Notification
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mail;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;
    using Idera.SQLdm.ManagementService.Monitoring;
    using Microsoft.ApplicationBlocks.Data;
    using Vim25Api;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    public class NotificationManager
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationManager");

        private ReaderWriterLock sync = new ReaderWriterLock();

        private Dictionary<Guid, NotificationProviderContext> contexts;
        private Dictionary<Guid, NotificationRule> ruleDictionary;
        //START SQLDM 10.1 Barkha khatri 
        private const string AddSCOMAlertEventDefaultValuesStoredProcedure = "p_AddSCOMAlertEventDefaultValues";
        private const string ServiceStatusDefaultRule = "SQL Diagnostic Manager Service Status";
        string[] rankValueWithIndex = new string[2];
        MetricInfo metricInfo;
        MetricDefinition metricdefinition;
        //END SQLDM 10.1 Barkha khatri 
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotificationManager"/> class.
        /// </summary>
        public NotificationManager()
        {
            using (LOG.InfoCall("NotificationManager"))
            {
                contexts = new Dictionary<Guid, NotificationProviderContext>();
                ruleDictionary = new Dictionary<Guid, NotificationRule>();
                Initialize();
            }
        }

        private void Initialize()
        {
            using (LOG.InfoCall("Initialize"))
            {
                // force the metric iinfo map to initialize
                GetMetricInfo(Metric.AgentServiceStatus);

                Type providerInterface = typeof(INotificationProvider);
                foreach (Type type in Assembly.GetExecutingAssembly().GetExportedTypes())
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;

                    if (providerInterface.IsAssignableFrom(type))
                    {
                        NotificationProviderInfo.RegisterNotificationProviderType(type);
                    }
                }

                // Event Log and Tasks have no configuration and should always exist.  If they don't we need
                // to automatically add them...
                Guid eventLogProviderId = new EventLogDestination().ProviderID;
                Guid taskProviderId = new TaskDestination().ProviderID;
                Guid programProviderId = new ProgramDestination().ProviderID;
                Guid jobProviderId = new JobDestination().ProviderID;
                Guid sqlProviderId = new SqlDestination().ProviderID;
                Guid qmProviderId = new EnableQMDestination().ProviderID;
                Guid pulseProviderId = new PulseDestination().ProviderID;
                Guid paProviderId = new EnablePADestination().ProviderID;
                Guid qwProviderId = new EnableQWaitsDestination().ProviderID;
                Guid psProviderId = new PowerShellDestination().ProviderID;
                Guid scomProviderId = new SCOMAlertDestination().ProviderID;
                Guid scomEventProviderId = new SCOMEventDestination().ProviderID;

                bool haveEventLogProvider = false;
                bool haveTaskProvider = false;
                bool haveProgramProvider = false;
                bool haveJobProvider = false;
                bool haveSqlProvider = false;
                bool haveQMProvider = false;
                bool havePulseProvider = false;
                bool haveSMTPProvider = false;
                bool haveSNMPProvider = false;
                bool havePAProvider = false;
                bool haveQWProvider = false;
                bool havePSProvider = false;
                bool haveSCOMProvider = false;
                bool haveSCOMEventProvider = false;

                try
                {
                    sync.AcquireWriterLock(-1);
                    try
                    {
                        // dispose all existing contexts
                        foreach (
                            NotificationProviderContext context in
                                Collections.ToArray<NotificationProviderContext>(
                                    contexts.Values as ICollection<NotificationProviderContext>))
                        {
                            context.Dispose();
                        }
                        contexts.Clear();

                        // create new contexts
                        foreach (NotificationProviderInfo info in GetNotificationProviders())
                        {
                            NotificationProviderContext context = new NotificationProviderContext(info);
                            contexts.Add(info.Id, context);
                            if (info.Id == eventLogProviderId)
                                haveEventLogProvider = true;
                            else if (info.Id == taskProviderId)
                                haveTaskProvider = true;
                            else if (info.Id == programProviderId)
                                haveProgramProvider = true;
                            else if (info.Id == jobProviderId)
                                haveJobProvider = true;
                            else if (info.Id == sqlProviderId)
                                haveSqlProvider = true;
                            else if (info.Id == qmProviderId)
                                haveQMProvider = true;
                            else if (info.Id == qwProviderId)
                                haveQWProvider = true;
                            else if (info.Id == pulseProviderId)
                            {
                                havePulseProvider = true;
                                LOG.Info("Initializing news feed interfaces");
                                try
                                {
                                    WebClient.WebClient.RefreshPulseRegistration(info as PulseNotificationProviderInfo);
                                }
                                catch (Exception p)
                                {
                                    LOG.Error("Error initializing news feed: ", p);
                                }
                            }
                            else if (info is SnmpNotificationProviderInfo)
                                haveSNMPProvider = true;
                            else if (info is SmtpNotificationProviderInfo)
                                haveSMTPProvider = true;
                            else if (info.Id == paProviderId)
                                havePAProvider = true;
                            else if (info.Id == psProviderId)
                                havePSProvider = true;
                            else if (info.Id == scomProviderId)
                                haveSCOMProvider = true;
                            else if (info.Id == scomEventProviderId)
                                haveSCOMEventProvider = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error materializing notification providers", e);
                    }

                    if (!haveEventLogProvider)
                    {
                        LOG.Info("Adding Event Log Action Provider to the repository");
                        EventLogNotificationProviderInfo npi = new EventLogNotificationProviderInfo(true);
                        npi.Id = eventLogProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveTaskProvider)
                    {
                        LOG.Info("Adding Task Action Provider to the repository");
                        TaskNotificationProviderInfo npi = new TaskNotificationProviderInfo(true);
                        npi.Id = taskProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveProgramProvider)
                    {
                        LOG.Info("Adding Program Action Provider to the repository");
                        ProgramNotificationProviderInfo npi = new ProgramNotificationProviderInfo(true);
                        npi.Id = programProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveJobProvider)
                    {
                        LOG.Info("Adding Job Action Provider to the repository");
                        JobNotificationProviderInfo npi = new JobNotificationProviderInfo(true);
                        npi.Id = jobProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveSqlProvider)
                    {
                        LOG.Info("Adding SQL Action Provider to the repository");
                        SqlNotificationProviderInfo npi = new SqlNotificationProviderInfo(true);
                        npi.Id = sqlProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveQMProvider)
                    {
                        LOG.Info("Adding Enable QM Provider to the repository");
                        EnableQMNotificationProviderInfo npi = new EnableQMNotificationProviderInfo(true);
                        npi.Id = qmProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveSMTPProvider)
                    {
                        LOG.Info("Adding SMTP Provider to the repository");
                        SmtpNotificationProviderInfo npi = new SmtpNotificationProviderInfo(true);
                        npi.Name = "Email (SMTP) Provider";
                        AddNotificationProvider(npi, true);
                    }
                    if (!haveSNMPProvider)
                    {
                        LOG.Info("Adding SNMP Provider to the repository");
                        SnmpNotificationProviderInfo npi = new SnmpNotificationProviderInfo(true);
                        npi.Name = "Network Management (SNMP) Trap Message Provider";
                        AddNotificationProvider(npi, true);
                    }
                    if (!havePAProvider)
                    {
                        LOG.Info("Adding Enable PA Provider to the repository");
                        EnablePANotificationProviderInfo npi = new EnablePANotificationProviderInfo(true);
                        npi.Id = paProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveQWProvider)
                    {
                        LOG.Info("Adding Enable Query Waits Provider to the repository");
                        EnableQWaitsNotificationProviderInfo npi = new EnableQWaitsNotificationProviderInfo(true);
                        npi.Id = qwProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    //Alert Power Shell Provider
                    //SQLdm 10.1 Srishti Purohit
                    if (!havePSProvider)
                    {
                        LOG.Info("Adding Power Shell Provider to the repository");
                        PowerShellNotificationProviderInfo npi = new PowerShellNotificationProviderInfo(true);
                        npi.Id = psProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    //SCOM Alert Response Action
                    //SQLdm 10.1 (Srishti Purohit)
                    if (!haveSCOMProvider)
                    {
                        LOG.Info("Adding SCOM alert Provider to the repository");
                        SCOMAlertNotificationProviderInfo npi = new SCOMAlertNotificationProviderInfo(true);
                        npi.Id = scomProviderId;
                        AddNotificationProvider(npi, false);
                    }
                    if (!haveSCOMEventProvider)
                    {
                        LOG.Info("Adding SCOM event Provider to the repository");
                        SCOMEventNotificationProviderInfo npi = new SCOMEventNotificationProviderInfo(true);
                        npi.Id = scomEventProviderId;
                        AddNotificationProvider(npi, false);
                    }

                    LOG.Info("NotificationProvider instances created");

                    try
                    {
                        // remove existing cached rules
                        ruleDictionary.Clear();
                        bool fortWorthUpgradesComplete =
                            FortWorthActionUpgradesCompleted(ManagementServiceConfiguration.ConnectionString);
                        if (!fortWorthUpgradesComplete)
                            LOG.Info("Doing 6.1 action rule upgrades");
                        // cache rules
                        foreach (NotificationRule rule in GetNotificationRules())
                        {
                            if (!fortWorthUpgradesComplete)
                            {
                                string oldName = String.Empty;
                                try
                                {
                                    if (rule.Description.Equals("Missed Collection Service Heartbeat",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        oldName = rule.Description;
                                        rule.Description = ServiceStatusDefaultRule;
                                        UpdateNotificationRule(rule);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LOG.ErrorFormat("Failed to upgrade action rule name from '{0}' to '{1}': {2}",
                                                    oldName, rule.Description, e);
                                }
                            }

                            if (ruleDictionary.ContainsKey(rule.Id))
                                ruleDictionary.Remove(rule.Id);
                            ruleDictionary.Add(rule.Id, rule);
                            // log rules that have invalid destinations
                            foreach (NotificationDestinationInfo destination in rule.Destinations)
                            {
                                NotificationProviderContext providerContext = this[destination.ProviderID];
                                if (providerContext == null)
                                    LOG.DebugFormat("Notification rule: {0} has an invalid destination id={1}",
                                                    rule.Description,
                                                    destination.ProviderID);
                                else if (!providerContext.IsEnabled)
                                    LOG.DebugFormat("Notification rule: {0} has a disabled destination id={1}",
                                                    rule.Description,
                                                    destination.ProviderID);
                            }
                        }

                        if (!fortWorthUpgradesComplete)
                            SetFortWorthActionUpgradesComplete(ManagementServiceConfiguration.ConnectionString);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error materializing notification rules", e);
                    }
                }
                finally
                {
                    sync.ReleaseWriterLock();
                }
            }
        }

        internal NotificationProviderContext GetPulseNotificationContext()
        {
            NotificationProviderContext result = null;

            sync.AcquireReaderLock(-1);
            try
            {
                foreach (
                    NotificationProviderContext context in
                        Collections.ToArray<NotificationProviderContext>(contexts.Values as ICollection<NotificationProviderContext>))
                {
                    if (context.Provider is Idera.SQLdm.ManagementService.Notification.Providers.PulseNotificationProvider)
                    {
                        result = context;
                        break;
                    }
                }
            }
            finally
            {
                sync.ReleaseReaderLock();
            }

            return result;
        }

        public static List<MetricThresholdEntry> GetStartingMetricThresholds(int userViewId)
        {
            List<MetricThresholdEntry> items = new List<MetricThresholdEntry>();

            // All the service status alerts use the same warning and critical states.
            Threshold info =
                new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            Threshold warning =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.ContinuePending,
                                        ServiceState.NotInstalled,
                                        ServiceState.Paused,
                                        ServiceState.PausePending,
                                        ServiceState.StartPending,
                                        ServiceState.StopPending,
                                        ServiceState.TruncatedFunctionalityAvailable
                              ));
            Threshold error =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.UnableToMonitor,
                                        ServiceState.UnableToConnect,
                                        ServiceState.Stopped));

            // SQL Server Status
            items.Add(new MetricThresholdEntry(userViewId, Metric.SqlServiceStatus, warning, error, info));

            // Other services status - no paused status possible
            info =
                new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.ContinuePending,
                                        ServiceState.PausePending,
                                        ServiceState.NotInstalled,
                                        ServiceState.StartPending,
                                        ServiceState.StopPending,
                                        ServiceState.TruncatedFunctionalityAvailable
                              ));
            error =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.UnableToMonitor,
                                        ServiceState.UnableToConnect,
                                        ServiceState.Stopped));
            items.Add(new MetricThresholdEntry(userViewId, Metric.AgentServiceStatus, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.DtcServiceStatus, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.FullTextServiceStatus, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 75.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.SQLCPUUsagePct, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.ReorganisationPct, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 85.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 90.0f);
            error = new Threshold(true, Threshold.Operator.GE, 95.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.SQLMemoryUsagePct, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 80.0f);
            error = new Threshold(true, Threshold.Operator.GE, 95.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.UserConnectionPct, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 0.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 1.0f);
            error = new Threshold(true, Threshold.Operator.GE, 5.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.NonDistributedTrans, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 1.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 5.0f);
            error = new Threshold(true, Threshold.Operator.GE, 15.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.NonSubscribedTransNum, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 3.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 5.0f);
            error = new Threshold(true, Threshold.Operator.GE, 10.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OldestOpenTransMinutes, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 3.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 5.0f);
            error = new Threshold(true, Threshold.Operator.GE, 10.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.IndexRowHits, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 12.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 24.0f);
            error = new Threshold(true, Threshold.Operator.GE, 48.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.FullTextRefreshHours, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 15.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 30.0f);
            error = new Threshold(true, Threshold.Operator.GE, 60.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.NonSubscribedTransTime, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 1500.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 2000.0f);
            error = new Threshold(true, Threshold.Operator.GE, 5000.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.ServerResponseTime, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 75.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSCPUUsagePct, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSDiskPhysicalDiskTimePct, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSDiskPhysicalDiskTimePctPerDisk, warning, error, info));

            //         
            MetricThresholdEntry mte = new MetricThresholdEntry(userViewId, Metric.OSUserCPUUsagePct, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);
            mte = new MetricThresholdEntry(userViewId, Metric.OSCPUPrivilegedTimePct, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);
            //

            // disabled by default -- 10/4/2007
            info = new Threshold(false, Threshold.Operator.GE, 60.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 75.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            mte = new MetricThresholdEntry(userViewId, Metric.DataUsedPct, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);
            // disabled by default -- 10/4/2007
            mte = new MetricThresholdEntry(userViewId, Metric.LogUsedPct, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            info = new Threshold(false, Threshold.Operator.GE, 1.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 3.0f);
            error = new Threshold(true, Threshold.Operator.GE, 5.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSCPUProcessorQueueLength, warning, error, info));

            // disabled by default
            mte = new MetricThresholdEntry(userViewId, Metric.OSDiskAverageDiskQueueLength, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            items.Add(new MetricThresholdEntry(userViewId, Metric.OSDiskAverageDiskQueueLengthPerDisk, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 100.0f);
            error = new Threshold(true, Threshold.Operator.GE, 1000.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSMemoryPagesPerSecond, warning, error, info));


            info = new Threshold(false, Threshold.Operator.GE, 85.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 90.0f);
            error = new Threshold(true, Threshold.Operator.GE, 95.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSMemoryUsagePct, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 10.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 20.0f);
            error = new Threshold(true, Threshold.Operator.GE, 60.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.ResourceAlert, warning, error, info));

            info =
                new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(DBStatus.Loading, DBStatus.Offline, DBStatus.Recovering, DBStatus.PreRecovery));
            error =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(DBStatus.Suspect, DBStatus.Inaccessible, DBStatus.EmergencyMode));

            items.Add(new MetricThresholdEntry(userViewId, Metric.DatabaseStatus, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 70.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 80.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.DatabaseSizePct, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 65.0f);
            error = new Threshold(true, Threshold.Operator.GE, 80.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.TransLogSize, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 75.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSDiskFull, warning, error, info));

            // OS Metrics Status
            info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList());
            error = new Threshold(true, Threshold.Operator.EQ,
                    new Threshold.ComparableList(OSMetricsStatus.Disabled,
                                                 OSMetricsStatus.OLEAutomationUnavailable,
                                                 OSMetricsStatus.UnavailableDueToLightweightPooling,
                                                 OSMetricsStatus.WMIServiceUnreachable));
            items.Add(new MetricThresholdEntry(userViewId, Metric.OSMetricsStatus, warning, error, info));


            // Bombed jobs
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 1);
            error = new Threshold(false, Threshold.Operator.GE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.BombedJobs, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Long Jobs
            info = new Threshold(false, Threshold.Operator.GE, 20);
            warning = new Threshold(true, Threshold.Operator.GE, 50);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.LongJobs, warning, error, info);
            items.Add(mte);

            // Query Monitor
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 1);
            error = new Threshold(false, Threshold.Operator.GE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.QueryMonitorStatus, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // I/O Error
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(false, Threshold.Operator.GE, 1);
            error = new Threshold(true, Threshold.Operator.GE, 1);
            items.Add(new MetricThresholdEntry(userViewId, Metric.ReadWriteErrors, warning, error, info));

            // Blocking alert
            info = new Threshold(false, Threshold.Operator.GE, 10);
            warning = new Threshold(true, Threshold.Operator.GE, 20);
            error = new Threshold(true, Threshold.Operator.GE, 150);
            items.Add(new MetricThresholdEntry(userViewId, Metric.BlockingAlert, warning, error, info));

            info = new Threshold(false, Threshold.Operator.EQ, OptionStatus.Enabled);
            warning = new Threshold(false, Threshold.Operator.EQ, OptionStatus.Enabled);
            error = new Threshold(true, Threshold.Operator.EQ, OptionStatus.Enabled);
            items.Add(new MetricThresholdEntry(userViewId, Metric.CLRStatus, warning, error, info));

            // PR 10713 - remove maintenance mode as a configurable alert
            // items.Add(new MetricThresholdEntry(userViewId, Metric.MaintenanceMode, warning, error));

            info = new Threshold(false, Threshold.Operator.EQ, OptionStatus.Disabled);
            warning = new Threshold(true, Threshold.Operator.EQ, OptionStatus.Disabled);
            error = new Threshold(false, Threshold.Operator.EQ, OptionStatus.Disabled);
            items.Add(new MetricThresholdEntry(userViewId, Metric.AgentXPStatus, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.OLEAutomationStatus, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.WMIStatus, warning, error, info));
            items.Add(new MetricThresholdEntry(userViewId, Metric.SSConnectionProblem, warning, error, info)); // Added 12/12/4 - BMT

            info = new Threshold(false, Threshold.Operator.GE, 10);
            warning = new Threshold(true, Threshold.Operator.GE, 25);
            error = new Threshold(true, Threshold.Operator.GE, 50);
            mte = new MetricThresholdEntry(userViewId, Metric.ClientComputers, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(false, Threshold.Operator.GE, 5);
            error = new Threshold(true, Threshold.Operator.GE, 15);
            items.Add(new MetricThresholdEntry(userViewId, Metric.BlockedSessions, warning, error, info));

            info = new Threshold(false, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 20);
            mte = new MetricThresholdEntry(userViewId, Metric.LongJobsMinutes, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            AdvancedAlertConfigurationSettings errorLogDefault = new AdvancedAlertConfigurationSettings();
            errorLogDefault.LogIncludeRegexCritical = new string[] { "warning|!(cycle )errorlog|access_violation" };
            errorLogDefault.Metric = Metric.ErrorLog;
            info = new Threshold(false, Threshold.Operator.GE, 8);
            warning = new Threshold(true, Threshold.Operator.GE, 11);
            error = new Threshold(true, Threshold.Operator.GE, 18);
            mte = new MetricThresholdEntry(userViewId, Metric.ErrorLog, warning, error, info);
            mte.Data = errorLogDefault;
            mte.IsEnabled = true;
            items.Add(mte);

            info = new Threshold(false, Threshold.Operator.LE, 3);
            warning = new Threshold(true, Threshold.Operator.LE, 2);
            error = new Threshold(true, Threshold.Operator.LE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.AgentLog, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Mirroring unsent log
            info = new Threshold(false, Threshold.Operator.GE, 24);
            warning = new Threshold(true, Threshold.Operator.GE, 48);
            error = new Threshold(true, Threshold.Operator.GE, 64);
            mte = new MetricThresholdEntry(userViewId, Metric.UnsentLogThreshold, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //mirroring unrestored log
            info = new Threshold(false, Threshold.Operator.GE, 24);
            warning = new Threshold(true, Threshold.Operator.GE, 48);
            error = new Threshold(true, Threshold.Operator.GE, 64);
            mte = new MetricThresholdEntry(userViewId, Metric.UnrestoredLog, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //mirroring oldest unsent transaction
            info = new Threshold(false, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 20);
            mte = new MetricThresholdEntry(userViewId, Metric.OldestUnsentMirroringTran, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //mirror commit overhead
            info = new Threshold(false, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 20);
            mte = new MetricThresholdEntry(userViewId, Metric.MirrorCommitOverhead, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Mirroring Status
            info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(MirroringMetrics.MirroringStateEnum.Suspended));
            error = new Threshold(true, Threshold.Operator.EQ,
                    new Threshold.ComparableList(MirroringMetrics.MirroringStateEnum.Disconnected,
                                                 MirroringMetrics.MirroringStateEnum.PendingFailover));
            items.Add(new MetricThresholdEntry(userViewId, Metric.MirroringSessionsStatus, warning, error, info));

            // Mirroring Non-preferred role
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 1);
            error = new Threshold(false, Threshold.Operator.GE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.MirroringSessionNonPreferredConfig, warning, error, info);
            items.Add(mte);

            // Mirroring Role Change
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 1);
            error = new Threshold(false, Threshold.Operator.GE, 1);
            items.Add(new MetricThresholdEntry(userViewId, Metric.MirroringSessionRoleChange, warning, error, info));

            // Mirroring Witness Connection
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(false, Threshold.Operator.GE, 1);
            error = new Threshold(true, Threshold.Operator.GE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.MirroringWitnessConnection, warning, error, info);

            items.Add(mte);

            // Page LIfe Expectancy
            info = new Threshold(false, Threshold.Operator.LE, 4800);
            warning = new Threshold(true, Threshold.Operator.LE, 3600);
            error = new Threshold(true, Threshold.Operator.LE, 1600);
            items.Add(new MetricThresholdEntry(userViewId, Metric.PageLifeExpectancy, warning, error, info));

            // Cluster Failover
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(false, Threshold.Operator.GE, 1);
            error = new Threshold(true, Threshold.Operator.GE, 1);
            items.Add(new MetricThresholdEntry(userViewId, Metric.ClusterFailover, warning, error, info));

            // Cluster Non-preferred role
            mte = new MetricThresholdEntry(userViewId, Metric.ClusterActiveNode, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Non distributed latency
            info = new Threshold(false, Threshold.Operator.GE, 15.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 30.0f);
            error = new Threshold(true, Threshold.Operator.GE, 60.0f);
            items.Add(new MetricThresholdEntry(userViewId, Metric.NonDistributedTransTime, warning, error, info));

            // Deadlock
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(false, Threshold.Operator.GE, 1);
            error = new Threshold(true, Threshold.Operator.GE, 1);
            items.Add(new MetricThresholdEntry(userViewId, Metric.Deadlock, warning, error, info));

            // Procedure Cache Hit Ratio
            info = new Threshold(false, Threshold.Operator.LE, 90);
            warning = new Threshold(true, Threshold.Operator.LE, 80);
            error = new Threshold(true, Threshold.Operator.LE, 65);
            mte = new MetricThresholdEntry(userViewId, Metric.ProcCacheHitRatio, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Average Disk Milliseconds Per Read
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 200);
            error = new Threshold(true, Threshold.Operator.GE, 500);
            mte = new MetricThresholdEntry(userViewId, Metric.AverageDiskMillisecondsPerRead, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Average Disk Milliseconds Per Transfer
            mte = new MetricThresholdEntry(userViewId, Metric.AverageDiskMillisecondsPerTransfer, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Average Disk Milliseconds Per Write
            mte = new MetricThresholdEntry(userViewId, Metric.AverageDiskMillisecondsPerWrite, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Disk Reads Per Second
            mte = new MetricThresholdEntry(userViewId, Metric.DiskReadsPerSecond, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Disk Transfers Per Second
            mte = new MetricThresholdEntry(userViewId, Metric.DiskTransfersPerSecond, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Disk Writes Per Second
            mte = new MetricThresholdEntry(userViewId, Metric.DiskWritesPerSecond, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Job Step Completion Status
            info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                    JobStepCompletionStatus.Retry,
                                    JobStepCompletionStatus.Cancelled
                              ));
            error =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                    JobStepCompletionStatus.Failed,
                                    JobStepCompletionStatus.Unknown
                              ));

            mte = new MetricThresholdEntry(userViewId, Metric.JobCompletion, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Version Store Generation Ratio
            info = new Threshold(false, Threshold.Operator.GE, 30);
            warning = new Threshold(true, Threshold.Operator.GE, 50);
            error = new Threshold(true, Threshold.Operator.GE, 80);
            mte = new MetricThresholdEntry(userViewId, Metric.VersionStoreGenerationRatio, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // Version Store Size
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 500);
            error = new Threshold(true, Threshold.Operator.GE, 1000);
            mte = new MetricThresholdEntry(userViewId, Metric.VersionStoreSize, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Long Running Version Store Transaction
            info = new Threshold(true, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 15);
            mte = new MetricThresholdEntry(userViewId, Metric.LongRunningVersionStoreTransaction, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Session Tempdb Space Usage
            info = new Threshold(false, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 150);
            mte = new MetricThresholdEntry(userViewId, Metric.SessionTempdbSpaceUsage, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Tempdb Contention
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 250);
            error = new Threshold(true, Threshold.Operator.GE, 500);
            mte = new MetricThresholdEntry(userViewId, Metric.TempdbContention, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Log File Autogrow
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(true, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.LogFileAutogrow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Data  File Autogrow
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(true, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.DataFileAutogrow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Config Change
            info = new Threshold(true, Threshold.Operator.EQ, true);
            warning = new Threshold(false, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.VmConfigChange, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Host Server Change
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(true, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.VmHostServerChange, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // VM CPU Utilization
            info = new Threshold(false, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 75);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.VmCPUUtilization, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // ESX CPU Utilization
            info = new Threshold(false, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 75);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.VmESXCPUUtilization, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Memory Utilization
            info = new Threshold(false, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 75);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.VmMemoryUtilization, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // VM ESX Memory Utilization
            info = new Threshold(false, Threshold.Operator.GE, 70);
            warning = new Threshold(true, Threshold.Operator.GE, 80);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.VmESXMemoryUsage, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM CPU Ready Time 
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 250);
            error = new Threshold(true, Threshold.Operator.GE, 500);
            mte = new MetricThresholdEntry(userViewId, Metric.VmCPUReadyWaitTime, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Ballooned Memory
            info = new Threshold(false, Threshold.Operator.GE, 500);
            warning = new Threshold(true, Threshold.Operator.GE, 1000);
            error = new Threshold(true, Threshold.Operator.GE, 2000);
            mte = new MetricThresholdEntry(userViewId, Metric.VmReclaimedMemory, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Memory Swapping Detected
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(true, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.VmMemorySwapDelayDetected, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // VM Memory Swapping Detected at the ESX Server
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(true, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.VmESXMemorySwapDetected, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // VM Resource Limits Detected
            info = new Threshold(true, Threshold.Operator.EQ, true);
            warning = new Threshold(false, Threshold.Operator.EQ, true);
            error = new Threshold(false, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.VmResourceLimits, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            // VM Power State
            info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            error = new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        VirtualMachinePowerState.suspended,
                                        VirtualMachinePowerState.poweredOff));
            mte = new MetricThresholdEntry(userViewId, Metric.VmPowerState, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // ESX Power State
            info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning = new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        HostSystemPowerState.unknown));
            error = new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        HostSystemPowerState.poweredOff,
                                        HostSystemPowerState.standBy));
            mte = new MetricThresholdEntry(userViewId, Metric.VmESXPowerState, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            // Database Size (MB)
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 500);
            error = new Threshold(true, Threshold.Operator.GE, 1000);
            mte = new MetricThresholdEntry(userViewId, Metric.DatabaseSizeMb, warning, error, info) { IsEnabled = false };
            items.Add(mte);

            // Log File Size (MB)
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 500);
            error = new Threshold(true, Threshold.Operator.GE, 1000);
            mte = new MetricThresholdEntry(userViewId, Metric.TransLogSizeMb, warning, error, info) { IsEnabled = false };
            items.Add(mte);

            // OS Disk Free Space (MB)
            info = new Threshold(false, Threshold.Operator.LE, 1000);
            warning = new Threshold(true, Threshold.Operator.LE, 500);
            error = new Threshold(true, Threshold.Operator.LE, 100);
            mte = new MetricThresholdEntry(userViewId, Metric.OsDiskFreeMb, warning, error, info) { IsEnabled = false };
            items.Add(mte);

            //// SQLsafe backup operation code
            //info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            //warning = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.SuccessWithWarning,
            //                                                                                   OperationStatusCode.Cancelled,
            //                                                                                   OperationStatusCode.Skipped));
            //error = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.Error,
            //                                                                                   OperationStatusCode.Missed,
            //                                                                                   OperationStatusCode.Halted));
            //mte = new MetricThresholdEntry(userViewId, Metric.SSBackupOperation, warning, error, info) { IsEnabled = false };
            //items.Add(mte);

            //// SQLsafe restore operation code
            //info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            //warning = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.SuccessWithWarning,
            //                                                                                   OperationStatusCode.Cancelled,
            //                                                                                   OperationStatusCode.Skipped));
            //error = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.Error,
            //                                                                                   OperationStatusCode.Missed,
            //                                                                                   OperationStatusCode.Halted));
            //mte = new MetricThresholdEntry(userViewId, Metric.SSRestoreOperation, warning, error, info) { IsEnabled = false };
            //items.Add(mte);

            //// SQLsafe defrag operation code
            //info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            //warning = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.SuccessWithWarning,
            //                                                                                   OperationStatusCode.Cancelled,
            //                                                                                   OperationStatusCode.Skipped));
            //error = new Threshold(true, Threshold.Operator.EQ, new Threshold.ComparableList(OperationStatusCode.Error,
            //                                                                                   OperationStatusCode.Missed,
            //                                                                                   OperationStatusCode.Halted));
            //mte = new MetricThresholdEntry(userViewId, Metric.SSDefragOperation, warning, error, info) { IsEnabled = false };
            //items.Add(mte);

            // SQLsafe connection status metric added earlier with the WMIStatus metric... 

            // Availability Group Role Change 
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(false, Threshold.Operator.EQ, true);
            error = new Threshold(true, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnAvailabilityGroupRoleChange, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Estimated Data Loss (time)
            info = new Threshold(false, Threshold.Operator.GE, 600);
            warning = new Threshold(true, Threshold.Operator.GE, 900);
            error = new Threshold(true, Threshold.Operator.GE, 1800);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnEstimatedDataLossTime, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Synchronization Health 
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(false, Threshold.Operator.EQ, true);
            error = new Threshold(true, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnSynchronizationHealthState, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Estimated Recovery time (seconds)
            info = new Threshold(false, Threshold.Operator.GE, 600);
            warning = new Threshold(true, Threshold.Operator.GE, 900);
            error = new Threshold(true, Threshold.Operator.GE, 1800);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnEstimatedRecoveryTime, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Synchronization Performance (seconds)
            info = new Threshold(false, Threshold.Operator.GE, 600);
            warning = new Threshold(true, Threshold.Operator.GE, 900);
            error = new Threshold(true, Threshold.Operator.GE, 1800);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnSynchronizationPerformance, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Log Send Queue Size (KB)
            info = new Threshold(false, Threshold.Operator.GE, 1024);
            warning = new Threshold(true, Threshold.Operator.GE, 4096);
            //(Barkha Khatri) SQLDM 19680 fix(changing lower limit of critical)
            error = new Threshold(true, Threshold.Operator.GE, 50000000);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnLogSendQueueSize, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Redo Queue Size (KB) 
            info = new Threshold(false, Threshold.Operator.GE, 1024);
            //SQLDM-26485 fix(changing lower warning and critical thresholds)

            warning = new Threshold(true, Threshold.Operator.GE, 25000000);
            error = new Threshold(true, Threshold.Operator.GE, 30000000);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnRedoQueueSize, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            // Redo Rate (KB/sec)
            info = new Threshold(false, Threshold.Operator.LE, 2000);
            warning = new Threshold(true, Threshold.Operator.LE, 1000);
            error = new Threshold(true, Threshold.Operator.LE, 500);
            mte = new MetricThresholdEntry(userViewId, Metric.AlwaysOnRedoRate, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            //SQLdm 8.6 (Ankit Srivastava) -- Added new metric for Preferred Node feature 
            // Preferred Node Unavailability
            info = new Threshold(false, Threshold.Operator.EQ, 1);
            warning = new Threshold(false, Threshold.Operator.EQ, 1);
            error = new Threshold(true, Threshold.Operator.EQ, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.PreferredNodeUnavailability, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            //start- SQLdm 9.0 (Ankit Srivastava) -- Added new metric for Grooming Timed Out
            //SQLdm Repository Grooming Timed Out
            info = new Threshold(false, Threshold.Operator.EQ, 1);
            warning = new Threshold(false, Threshold.Operator.EQ, 1);
            error = new Threshold(true, Threshold.Operator.EQ, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.RepositoryGroomingTimedOut, warning, error, info) { IsEnabled = true };
            items.Add(mte);
            //end- SQLdm 9.0 (Ankit Srivastava) -- Added new metric for Grooming Timed Out

            //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added starting threshold values for metrices
            // Filegroup space full (Percent)
            info = new Threshold(false, Threshold.Operator.GE, 50.0f);
            warning = new Threshold(true, Threshold.Operator.GE, 75.0f);
            error = new Threshold(true, Threshold.Operator.GE, 90.0f);
            mte = new MetricThresholdEntry(userViewId, Metric.FilegroupSpaceFullPct, warning, error, info);
            items.Add(mte);

            // Filegroup space full (Size)
            info = new Threshold(false, Threshold.Operator.GE, 100);
            warning = new Threshold(true, Threshold.Operator.GE, 500);
            error = new Threshold(true, Threshold.Operator.GE, 1000);
            mte = new MetricThresholdEntry(userViewId, Metric.FilegroupSpaceFullSize, warning, error, info) { IsEnabled = false };
            items.Add(mte);
            //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added starting threshold values for metrices

            //START : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add new service status alerts
            info =
                new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
            warning =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.ContinuePending,
                                        ServiceState.NotInstalled,
                                        ServiceState.Paused,
                                        ServiceState.PausePending,
                                        ServiceState.StartPending,
                                        ServiceState.StopPending,
                                        ServiceState.TruncatedFunctionalityAvailable));
            error =
                new Threshold(true, Threshold.Operator.EQ,
                              new Threshold.ComparableList(
                                        ServiceState.UnableToMonitor,
                                        ServiceState.UnableToConnect,
                                        ServiceState.Stopped));
            mte = new MetricThresholdEntry(userViewId, Metric.SQLBrowserServiceStatus, warning, error, info) { IsEnabled = false };
            items.Add(mte);

            mte = new MetricThresholdEntry(userViewId, Metric.SQLActiveDirectoryHelperServiceStatus, warning, error, info) { IsEnabled = false };
            items.Add(mte);
            //END : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add new service status alerts

            //START: SQLdm 10.0 (Vandana Gogna) - Database Last Backup
            info = new Threshold(false, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 30);
            mte = new MetricThresholdEntry(userViewId, Metric.DatabaseLastBackupDate, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);
            //END: SQLdm 10.0 (Vandana Gogna) - Database Last Backup
            //NITOR START ADDING

            //Average Data IO Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 60);
            error = new Threshold(true, Threshold.Operator.GE, 80);
            mte = new MetricThresholdEntry(userViewId, Metric.AverageDataIOPercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Average Log Write Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 60);
            error = new Threshold(true, Threshold.Operator.GE, 80);
            mte = new MetricThresholdEntry(userViewId, Metric.AverageLogWritePercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Max Worker Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 60);
            error = new Threshold(true, Threshold.Operator.GE, 80);
            mte = new MetricThresholdEntry(userViewId, Metric.MaxWorkerPercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Read Latency Low
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 20);
            error = new Threshold(true, Threshold.Operator.GE, 12);
            mte = new MetricThresholdEntry(userViewId, Metric.ReadLatencyLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Write Latency Low
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 10);
            error = new Threshold(true, Threshold.Operator.GE, 6);
            mte = new MetricThresholdEntry(userViewId, Metric.WriteLatencyLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Max Session Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 60);
            error = new Threshold(true, Threshold.Operator.GE, 80);
            mte = new MetricThresholdEntry(userViewId, Metric.MaxSessionPercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Service Tier Changes
            info = new Threshold(false, Threshold.Operator.EQ, true);
            warning = new Threshold(false, Threshold.Operator.EQ, true);
            error = new Threshold(true, Threshold.Operator.EQ, true);
            mte = new MetricThresholdEntry(userViewId, Metric.ServiceTierChanges, warning, error, info) { IsEnabled = true };
            items.Add(mte);

            //Database Average Memory Usage Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 70);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.DatabaseAverageMemoryUsagePercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //In-Memory Storage Usage Percent
            info = new Threshold(false, Threshold.Operator.GE, 0);
            warning = new Threshold(true, Threshold.Operator.GE, 70);
            error = new Threshold(true, Threshold.Operator.GE, 90);
            mte = new MetricThresholdEntry(userViewId, Metric.InMemoryStorageUsagePercent, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //START 5.4.2
            //New Average Data IO Percent
            info = new Threshold(true, Threshold.Operator.LE, 25);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.AverageDataIOPercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //New Average Log Write Percent
            info = new Threshold(true, Threshold.Operator.LE, 25);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.AverageLogWritePercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //New Max Worker Percent
            info = new Threshold(true, Threshold.Operator.LE, 20);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.MaxWorkerPercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

           

            //New Max Session Percent
            info = new Threshold(true, Threshold.Operator.LE, 25);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.MaxSessionPercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //New Database Average Memory Usage Percent
            info = new Threshold(true, Threshold.Operator.LE, 25);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.DatabaseAverageMemoryUsagePercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //New In-Memory Storage Usage Percent
            info = new Threshold(true, Threshold.Operator.LE, 25);
            warning = new Threshold(false, Threshold.Operator.LE, 0);
            error = new Threshold(false, Threshold.Operator.LE, 0);
            mte = new MetricThresholdEntry(userViewId, Metric.InMemoryStorageUsagePercentLow, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //END 5.4.2

            //CPU Credit Balance
            info = new Threshold(false, Threshold.Operator.LE, 1000);
            warning = new Threshold(true, Threshold.Operator.LE, 55);
            error = new Threshold(true, Threshold.Operator.LE, 32);
            mte = new MetricThresholdEntry(userViewId, Metric.CPUCreditBalance, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //CPU Credit Balance High
            info = new Threshold(false, Threshold.Operator.LE, 1000);
            warning = new Threshold(true, Threshold.Operator.LE, 32);
            error = new Threshold(true, Threshold.Operator.LE, 55);
            mte = new MetricThresholdEntry(userViewId, Metric.CPUCreditBalanceHigh, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);


            //CPU Credit Usage
            info = new Threshold(false, Threshold.Operator.LE, 1000);
            warning = new Threshold(true, Threshold.Operator.LE, 55);
            error = new Threshold(true, Threshold.Operator.LE, 32);
            mte = new MetricThresholdEntry(userViewId, Metric.CPUCreditUsage, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Disk Queue Depth
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 2);
            error = new Threshold(true, Threshold.Operator.GE, 4);
            mte = new MetricThresholdEntry(userViewId, Metric.DiskQueueDepth, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Read Latency
            info = new Threshold(false, Threshold.Operator.GE, 5);
            warning = new Threshold(true, Threshold.Operator.GE, 12);
            error = new Threshold(true, Threshold.Operator.GE, 20);
            mte = new MetricThresholdEntry(userViewId, Metric.ReadLatency, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);

            //Read Throughput
            info = new Threshold(false, Threshold.Operator.LE, 999);
            warning = new Threshold(true, Threshold.Operator.LE, 5);
            error = new Threshold(true, Threshold.Operator.LE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.ReadThroughput, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Swap Usage
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 50);
            error = new Threshold(true, Threshold.Operator.GE, 100);
            mte = new MetricThresholdEntry(userViewId, Metric.SwapUsage, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Write Latency
            info = new Threshold(false, Threshold.Operator.GE, 1);
            warning = new Threshold(true, Threshold.Operator.GE, 6);
            error = new Threshold(true, Threshold.Operator.GE, 10);
            mte = new MetricThresholdEntry(userViewId, Metric.WriteLatency, warning, error, info);
            mte.IsEnabled = true;
            items.Add(mte);


            //Write Throughput
            info = new Threshold(false, Threshold.Operator.LE, 999);
            warning = new Threshold(true, Threshold.Operator.LE, 5);
            error = new Threshold(true, Threshold.Operator.LE, 1);
            mte = new MetricThresholdEntry(userViewId, Metric.WriteThroughput, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //SQL Server CPU Usage (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.SQLServerCPUUsagePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //User Connections (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.UserConnectionsPercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Latent replication transaction (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.LatentreplicationtransactionCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Unsubscribed Transactions (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.UnsubscribedTransactionsCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Oldest Open Transaction (Minutes)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OldestOpenTransactionMinutesBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //SQL Server Memory Usage (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.SQLServerMemoryUsagePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Unsubscribed Transactions (Seconds)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.UnsubscribedTransactionsSecondsBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //SQL Server Response Time (Milliseconds)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.SQLServerResponseTimeMillisecondsBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Memory Usage (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSMemoryUsagePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Paging (Per Second)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSPagingPerSecondBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Processor Time (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSProcessorTimePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Privileged Time (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSPrivilegedTimePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS User Time (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSUserTimePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Processor Queue Length (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSProcessorQueueLengthCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Disk Time (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSDiskTimePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //OS Average Disk Queue Length (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.OSAverageDiskQueueLengthCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Client Computers (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.ClientComputersCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Blocked Sessions (Count)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.BlockedSessionsCountBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //SQL Server Data Used (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.SQLServerDataUsedPercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //SQL Server Log Used (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.SQLServerLogUsedPercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Page Life Expectancy_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 120);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 50);
            mte = new MetricThresholdEntry(userViewId, Metric.PageLifeExpectancyBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Procedure Cache Hit Ratio_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 120);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 50);
            mte = new MetricThresholdEntry(userViewId, Metric.ProcedureCacheHitRatioBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);


            //VM CPU Usage (Percent)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.VMCPUUsagePercentBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);



            //Availability Group Estimated Data Loss (Seconds)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupEstimatedDataLossSecondsBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);


            //Availability Group Estimated Recovery Time (Seconds)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupEstimatedRecoveryTimeSecondsBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);


            //Availability Group Synchronization Performance (Seconds)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupSynchronizationPerformanceSecondsBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);


            //Availability Group Log Send Queue Size (KB)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupLogSendQueueSizeKBBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            //Availability Group Redo Queue Size (KB)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 50);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 120);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupRedoQueueSizeKBBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);


            //Availability Group Redo Rate (KB/sec)_Baseline
            info = new Threshold(true, Threshold.Operator.GE, 120);
            warning = new Threshold(true, Threshold.Operator.GE, 100);
            error = new Threshold(true, Threshold.Operator.GE, 50);
            mte = new MetricThresholdEntry(userViewId, Metric.AvailabilityGroupRedoRateKBsecBaseline, warning, error, info);
            mte.IsEnabled = false;
            items.Add(mte);

            return items;


            //END
        }

        public static void InsertStartingNotificationRules(SqlTransaction xa)
        {

            foreach (NotificationRule rule in NotificationManager.GetStartingNotificationRules())
            {
                try
                {
                    XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(NotificationRule));

                    using (SqlCommand command = SqlHelper.CreateCommand(xa.Connection, "p_AddNotificationRule"))
                    {
                        command.Transaction = xa;

                        string xml;

                        StringBuilder buffer = new StringBuilder();
                        using (XmlWriter writer = XmlWriter.Create(buffer))
                        {
                            serializer.Serialize(writer, rule);
                            writer.Flush();
                        }
                        xml = buffer.ToString();
                        SqlHelper.AssignParameterValues(command.Parameters, xml, DBNull.Value);
                        command.ExecuteNonQuery();
                        object id = command.Parameters["@ReturnRuleID"].Value;
                        if (id is Guid)
                            rule.Id = (Guid)id;
                        //SQLDM 10.1 (Barkha Khatri) SCOM feature--after rule is added to NotificationRule table add values to SCOMALertEvent table
                        if (rule.installationAction != null)
                        {
                            rule.installationAction(xa, rule.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // could happen as a result of adding a second management service
                    LOG.Debug("InsertStartingNotificationRules :" + ex.Message);
                }
            }

        }

        /// <summary>
        /// SQLDM 10.1 (Barkha Khatri) SCOM feature- adding default values to SCOMALertEvent table 
        /// </summary>
        /// <param name="ruleId"></param>
        internal static void InsertDefaultValuesInSCOMAlertEventTable(SqlTransaction xa, Guid ruleId)
        {
            try
            {
                using (SqlCommand command = SqlHelper.CreateCommand(xa.Connection, AddSCOMAlertEventDefaultValuesStoredProcedure))
                {
                    command.Transaction = xa;
                    SqlHelper.AssignParameterValues(command.Parameters, ruleId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LOG.Debug("InsertDefaultValuesInSCOMAlertEventTable: Exception " + ex.Message);
            }

        }


        private static List<NotificationRule> GetStartingNotificationRules()
        {
            List<NotificationRule> items = new List<NotificationRule>();
            try
            {
                NotificationRule rule = new NotificationRule();

                rule.Enabled = true;
                rule.Description = ServiceStatusDefaultRule;
                rule.Destinations.Add(new EventLogDestination());
                rule.AddMetric(Metric.SQLdmCollectionServiceStatus);
                rule.StateComparison = new MetricStateRule();
                items.Add(rule);


                //START SQLDM 10.1 Barkha khatri -- adding SCOM default rule
                rule = CreateSCOMDefaultNotificationRule();
                items.Add(rule);
                //END SQLDM 10.1 Barkha khatri -- adding SCOM default rule
            }
            catch (Exception ex)
            {
                LOG.Debug("GetStartingNotificationRules: " + ex.Message);
            }
            return items;
        }

        /// <summary>
        /// SQLDM 10.1 (Barkha Khatri) SCOM feature-- Creating default SCOM rule to send all alerts as Events to SCOM
        /// </summary>
        /// <returns></returns>
        public static NotificationRule CreateSCOMDefaultNotificationRule()
        {
            NotificationRule rule = new NotificationRule();
            rule.Enabled = true;
            rule.Description = Idera.SQLdm.Common.Constants.SCOMDefaultRuleDescription;
            rule.Destinations.Add(new SCOMEventDestination());
            rule.MetricIDs = null;
            rule.StateComparison = new MetricStateRule();
            rule.installationAction += InsertDefaultValuesInSCOMAlertEventTable;
            return rule;
        }
        internal NotificationProviderContext this[Guid providerId]
        {
            get
            {
                NotificationProviderContext result = null;
                sync.AcquireReaderLock(-1);
                try
                {
                    contexts.TryGetValue(providerId, out result);
                }
                finally
                {
                    sync.ReleaseReaderLock();
                }
                return result;
            }
            set
            {
                sync.AcquireWriterLock(-1);
                try
                {
                    if (contexts.ContainsKey(providerId))
                        contexts.Remove(providerId);
                    contexts.Add(providerId, value);
                }
                finally
                {
                    sync.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// Called when metric info is updated (done elsewhere) so that we can cache bits of the object
        /// for use in notifications.
        /// </summary>
        /// <param name="metricInfo"></param>
        public void CacheMetricInfo(MetricInfo metricInfo)
        {
            MetricInfo info = GetMetricInfo(metricInfo.Metric);
            lock (MetricInfo.MetricInfoMap)
            {
                if (info != null)
                    MetricInfo.MetricInfoMap.Remove(metricInfo.Metric);
                MetricInfo.MetricInfoMap.Add(metricInfo.Metric, metricInfo);
            }
        }

        public MetricInfo GetMetricInfo(Metric metric)
        {
            MetricDefinitions definitions = Management.GetMetricDefinitions();
            SharedMetricDefinitions.MetricDefinitions = definitions;

            MetricInfo result = null;
            Dictionary<Metric, MetricInfo> metricInfoDictionary = MetricInfo.MetricInfoMap;

            lock (metricInfoDictionary)
            {
                if (metricInfoDictionary.Count == 0)
                {
                    foreach (MetricInfo info in RepositoryHelper.GetMetricInfo(ManagementServiceConfiguration.ConnectionString).Values)
                    {
                        metricInfoDictionary.Add(info.Metric, info);
                    }
                }

                metricInfoDictionary.TryGetValue(metric, out result);
            }

            return result;
        }

        /// <summary>
        /// Gets the notification providers.
        /// </summary>
        /// <returns></returns>
        public IList<NotificationProviderInfo> GetNotificationProviders()
        {
            List<NotificationProviderInfo> result = new List<NotificationProviderInfo>();
            List<Type> types = NotificationProviderInfo.GetAvailableProviderTypes();

            using (LOG.InfoCall("GetNotificationProviders"))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(
                    ManagementServiceConfiguration.ConnectionString,
                    "p_GetNotificationProviders",
                    DBNull.Value))
                {
                    while (reader.Read())
                    {
                        NotificationProviderInfo npi = null;

                        Guid id = reader.GetGuid(0);
                        string typeName = reader.GetString(1);
                        string xml = reader.GetString(2);

                        Type providerType = null;

                        foreach (Type type in types)
                        {
                            if (type.Name == typeName)
                            {
                                providerType = type;
                                break;
                            }
                        }

                        if (providerType == null)
                        {
                            LOG.ErrorFormat("Unable to find notification provider type: {0}", typeName);
                            continue;
                        }

                        XmlSerializer serializer =
                            Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(providerType);
                        try
                        {
                            StringReader stream = new StringReader(xml);
                            using (XmlReader xmlReader = XmlReader.Create(stream))
                            {
                                npi = serializer.Deserialize(xmlReader) as NotificationProviderInfo;
                            }
                            npi.Id = id;
                            result.Add(npi);
                        }
                        catch (Exception)
                        {
                            LOG.ErrorFormat("Error deserializing notification provider info: Id={0} Type={1} Xml={2}",
                                            id, typeName, xml);
                        }
                    }
                }
            }
            return result;
        }

        private void ValidateProvider(NotificationProviderInfo newProviderInfo)
        {
            string newProviderName = newProviderInfo.Name.ToLower();
            Guid newProviderId = newProviderInfo.Id;
            // make sure the provider name is unique
            foreach (NotificationProviderInfo npi in GetNotificationProviders())
            {
                if (npi.Name.ToLower() == newProviderName && npi.Id != newProviderId)
                    throw new ManagementServiceException("Notification provider already exists with the same name.");
            }
        }

        public NotificationProviderInfo AddNotificationProvider(NotificationProviderInfo providerInfo, bool generateId)
        {
            ValidateProvider(providerInfo);

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddNotificationProvider"))
                {
                    Type providerType = providerInfo.GetType();
                    string typeName = providerType.Name;
                    string xml;

                    XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(providerType);
                    StringBuilder buffer = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(buffer))
                    {
                        serializer.Serialize(writer, providerInfo);
                        writer.Flush();
                    }
                    xml = buffer.ToString();

                    object providerId = DBNull.Value;
                    if (!generateId || providerInfo is PulseNotificationProviderInfo)
                        providerId = providerInfo.Id;

                    SqlHelper.AssignParameterValues(command.Parameters, typeName, xml, providerId);
                    command.ExecuteNonQuery();
                    object id = command.Parameters["@ReturnProviderId"].Value;
                    if (id is Guid)
                        providerInfo.Id = (Guid)id;
                }
            }

            NotificationProviderContext context = new NotificationProviderContext(providerInfo);
            this[providerInfo.Id] = context;

            if (providerInfo is PulseNotificationProviderInfo)
            {
                try
                {
                    RefreshPulseRegistration((PulseNotificationProviderInfo)providerInfo, 0, true);
                }
                catch (Exception e)
                {
                    ThreadPool.QueueUserWorkItem(delegate { RefreshPulseRegistration((PulseNotificationProviderInfo)providerInfo, 15000, false); });
                }
            }

            return providerInfo;
        }

        private static void RefreshPulseRegistration(PulseNotificationProviderInfo providerInfo, int waitTime, bool percolateException)
        {
            try
            {
                if (waitTime > 0)
                    Thread.Sleep(waitTime);

                WebClient.WebClient.RefreshPulseRegistration(providerInfo);
            }
            catch (Exception e)
            {
                LOG.Error("Error registering with news service: ", e);
                if (percolateException)
                    throw;
            }
        }

        public void DeleteNotificationProvider(Guid providerId)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(
                    ManagementServiceConfiguration.ConnectionString,
                    "p_DeleteNotificationProvider",
                    providerId);
            }
            catch (Exception e)
            {
                LOG.Error("Error deleteing notification provider info", e);
            }

            sync.AcquireWriterLock(-1);
            NotificationProviderContext context = null;
            if (contexts.TryGetValue(providerId, out context))
            {
                contexts.Remove(providerId);
            }
            sync.ReleaseWriterLock();

            if (context != null)
                context.Dispose();

            // remove all destinations referencing this provider
            foreach (NotificationRule rule in GetNotificationRules())
            {
                //TODO: remove all references to the provider & update rule
            }
        }


        /// <summary>
        /// Updates the notification provider.
        /// </summary>
        /// <param name="providerInfo">The provider.</param>
        /// <returns></returns>
        public bool UpdateNotificationProvider(NotificationProviderInfo providerInfo, bool updateRules)
        {
            ValidateProvider(providerInfo);

            Type providerType = providerInfo.GetType();
            string typeName = providerType.Name;

            if (updateRules && providerInfo is SmtpNotificationProviderInfo)
            {
                UpdateReferencedNotificatioRules((SmtpNotificationProviderInfo)providerInfo);
            }

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(providerType);
            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, providerInfo);
                writer.Flush();
            }
            string xml = buffer.ToString();

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateNotificationProvider"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, providerInfo.Id, typeName, xml);
                    command.ExecuteNonQuery();
                }
            }

            sync.AcquireWriterLock(-1);
            try
            {
                NotificationProviderContext context = null;
                if (contexts.TryGetValue(providerInfo.Id, out context))
                {
                    context.UpdateNotificationProvider(providerInfo);
                }
                else
                {
                    context = new NotificationProviderContext(providerInfo);
                    contexts.Add(providerInfo.Id, context);
                }
            }
            finally
            {
                sync.ReleaseWriterLock();
            }

            if (providerInfo is PulseNotificationProviderInfo)
            {
                try
                {
                    RefreshPulseRegistration((PulseNotificationProviderInfo)providerInfo, 0, true);
                }
                catch (Exception e)
                {
                    ThreadPool.QueueUserWorkItem(delegate { RefreshPulseRegistration((PulseNotificationProviderInfo)providerInfo, 15000, false); });
                }
            }

            return true;
        }

        private void UpdateReferencedNotificatioRules(SmtpNotificationProviderInfo smtpNotificationProviderInfo)
        {
            MailAddress fromAddress = new MailAddress(smtpNotificationProviderInfo.SenderAddress, smtpNotificationProviderInfo.SenderName);
            string fromAddressString = fromAddress.ToString();

            foreach (NotificationRule rule in Collections.ToArray(this.ruleDictionary.Values))
            {
                foreach (NotificationDestinationInfo destination in rule.Destinations)
                {
                    if (destination.ProviderID == smtpNotificationProviderInfo.Id)
                    {
                        SmtpDestination smtpDestination = (SmtpDestination)destination;
                        if (!smtpDestination.Equals(fromAddressString))
                        {
                            smtpDestination.From = fromAddressString;
                            UpdateNotificationRule(rule);
                        }
                    }
                }
            }
        }

        private void ValidateRule(NotificationRule newRule)
        {
            string newDesc = newRule.Description.ToLower();

            // make sure description is unique
            foreach (NotificationRule rule in GetNotificationRules())
            {
                if (rule.Description.ToLower() == newDesc && rule.Id != newRule.Id)
                    throw new ManagementServiceException("Notification rule already exists with the same name.");
            }
        }


        public NotificationRule AddNotificationRule(NotificationRule rule)
        {
            ValidateRule(rule);

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(NotificationRule));

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddNotificationRule"))
                {
                    string xml;

                    StringBuilder buffer = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(buffer))
                    {
                        serializer.Serialize(writer, rule);
                        writer.Flush();
                    }
                    xml = buffer.ToString();
                    SqlHelper.AssignParameterValues(command.Parameters, xml, DBNull.Value);
                    command.ExecuteNonQuery();
                    object id = command.Parameters["@ReturnRuleID"].Value;
                    if (id is Guid)
                        rule.Id = (Guid)id;

                    #region Change Log action

                    MAuditingEngine.Instance.LogAction(new NotificationRulesActions(AuditableActionType.AddAlertResponse));

                    #endregion Change Log action
                }
            }

            sync.AcquireWriterLock(-1);
            try
            {
                this.ruleDictionary[rule.Id] = rule;
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
            return rule;
        }

        /// <summary>
        /// SQLDM 10.1 (Barkha Khatri ) SCOM feature -- overloading for Post Install upgrade case 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public NotificationRule AddNotificationRule(SqlTransaction transaction, NotificationRule rule)
        {
            ValidateRule(rule);

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(NotificationRule));


            using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, "p_AddNotificationRule"))
            {
                command.Transaction = transaction;
                string xml;

                StringBuilder buffer = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(buffer))
                {
                    serializer.Serialize(writer, rule);
                    writer.Flush();
                }
                xml = buffer.ToString();
                SqlHelper.AssignParameterValues(command.Parameters, xml, DBNull.Value);
                command.ExecuteNonQuery();
                object id = command.Parameters["@ReturnRuleID"].Value;
                if (id is Guid)
                    rule.Id = (Guid)id;

                #region Change Log action

                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(AuditableActionType.AddAlertResponse));

                #endregion Change Log action
            }


            sync.AcquireWriterLock(-1);
            try
            {
                this.ruleDictionary[rule.Id] = rule;
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
            return rule;
        }

        public void UpdateNotificationRule(NotificationRule rule)
        {
            ValidateRule(rule);

            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(NotificationRule));

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateNotificationRule"))
                {
                    string xml;

                    StringBuilder buffer = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(buffer))
                    {
                        serializer.Serialize(writer, rule);
                        writer.Flush();
                    }
                    xml = buffer.ToString();
                    SqlHelper.AssignParameterValues(command.Parameters, rule.Id, xml);
                    command.ExecuteNonQuery();
                }

                #region Change Log action

                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(AuditableActionType.EditAlertResponse));

                #endregion Change Log action
            }

            sync.AcquireWriterLock(-1);
            try
            {
                if (this.ruleDictionary.ContainsKey(rule.Id))
                    this.ruleDictionary.Remove(rule.Id);
                this.ruleDictionary[rule.Id] = rule;
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
        }

        public void DeleteNotificationRule(Guid ruleId)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(
                    ManagementServiceConfiguration.ConnectionString,
                    "p_DeleteNotificationRule",
                    ruleId);

                #region Change Log action

                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(AuditableActionType.RemoveAlertResponse));

                #endregion Change Log action
            }
            catch (Exception e)
            {
                LOG.Error("Error deleteing notification rule", e);
            }

            sync.AcquireWriterLock(-1);
            try
            {
                if (ruleDictionary.ContainsKey(ruleId))
                {
                    ruleDictionary.Remove(ruleId);
                }
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
        }

        public static bool DoNotificationRulesExist(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) { return false; }

            bool hasRows = false;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString,
                            "p_GetNotificationRules", DBNull.Value))
            {
                hasRows = reader.HasRows;
            }
            return hasRows;
        }

        internal const string FORTWORTH_ACTIONS_UPGRADE_MARKER = "ActionsUpgradedTo6.1";
        internal static bool FortWorthActionUpgradesCompleted(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) { return false; }

            bool completed = false;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, System.Data.CommandType.StoredProcedure, "p_RepositoryInfo"))
            {
                while (!completed && reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        completed = String.Equals(reader.GetString(0), FORTWORTH_ACTIONS_UPGRADE_MARKER, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            return completed;
        }

        internal static void SetFortWorthActionUpgradesComplete(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) { return; }
            LOG.Info("Setting actions upgraded to 6.1 complete in repository");
            try
            {
                SqlHelper.ExecuteNonQuery(connectionString,
                                          System.Data.CommandType.Text,
                                          String.Format("insert into RepositoryInfo([Name],[Internal_Value],[Character_Value]) values('{0}', 1, null)", FORTWORTH_ACTIONS_UPGRADE_MARKER));
            }
            catch (Exception e)
            {
                LOG.Error("Error adding actions upgraded to 6.1 marker to repository: ", e);
            }
        }

        /// <summary>
        /// Gets the notification rules.
        /// </summary>
        /// <returns></returns>
        public IList<NotificationRule> GetNotificationRules()
        {
            List<NotificationRule> list = new List<NotificationRule>();

            XmlSerializer serializer =
                Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(NotificationRule),
                                                                            new Type[]
                                                                                {
                                                                                    typeof(EnableQMDestination),
                                                                                    typeof(EventLogDestination),
                                                                                    typeof(JobDestination),
                                                                                    typeof(ProgramDestination),
                                                                                    typeof(PulseDestination),
                                                                                    typeof(SmtpDestination),
                                                                                    typeof(SnmpDestination),
                                                                                    typeof(SqlDestination),
                                                                                    typeof(TaskDestination),
                                                                                    typeof(EnablePADestination),
                                                                                    typeof(EnableQWaitsDestination),
                                                                                    typeof(SCOMAlertDestination),
                                                                                    typeof(SCOMEventDestination)
                                                                                });
            using (SqlDataReader reader = SqlHelper.ExecuteReader(
                ManagementServiceConfiguration.ConnectionString,
                "p_GetNotificationRules",
                DBNull.Value))
            {
                while (reader.Read())
                {
                    Guid id = reader.GetGuid(0);
                    SqlString xml = reader.GetSqlString(1);
                    if (xml.IsNull)
                        continue;
                    NotificationRule info = null;
                    try
                    {
                        // deserialize the notification provider
                        StringReader stream = new StringReader(xml.Value);
                        using (XmlReader xmlReader = XmlReader.Create(stream))
                        {
                            info = (NotificationRule)serializer.Deserialize(xmlReader);
                            info.Id = id;
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("Exception deserializing NotificationRule ID=" + id.ToString(), ex);
                    }
                    list.Add(info);
                }
                return list;
            }
        }

        /// <summary>
        /// Saves the notification rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        public void SaveNotificationRule(NotificationRule rule)
        {
            if (rule.Id == default(Guid))
                AddNotificationRule(rule);
            else
                UpdateNotificationRule(rule);
        }

        public void TestNotification(NotificationContext context)
        {


        }

        public void Process(AlertableSnapshot refresh)
        {
            using (LOG.InfoCall("Process"))
            {
                if (refresh.Events == null)
                    return;

                MonitoredSqlServerState serverState =
                    Management.ScheduledCollection.GetCachedMonitoredSqlServer(refresh.Id);

                if (serverState != null)
                {
                    MonitoredSqlServerStateGraph stateGraph = serverState.StateGraph;

                    DateTime? serverTime = refresh.TimeStamp;
                    if (!serverTime.HasValue)
                    {
                        IEvent firstEvent = refresh.GetEvent(0);
                        if (firstEvent != null)
                            serverTime = firstEvent.OccuranceTime;
                    }

                    // don't process notifications unless this is the most current scheduled refresh
                    if (serverTime == null || serverTime < stateGraph.GetLastRefreshTime(refresh.GetType()))
                    {
                        LOG.InfoFormat("Skipping notifications for scheduled refresh: ({0} {1}) out of order.",
                                       serverState.WrappedServer.InstanceName, serverTime);
                        return;
                    }

                    MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();

                    List<Pair<int, IEvent>> filteredEvents = new List<Pair<int, IEvent>>();
                    foreach (IEvent ievent in refresh.Events)
                    {
                        if (!refresh.AlertableMetrics.Contains((Metric)ievent.MetricID))
                        {
                            LOG.DebugFormat("Skipping notification for {1} on {0} - event not raised by received snapshot of type {2}",
                                                   serverState.WrappedServer.InstanceName,
                                                   ievent,
                                                   refresh.GetType());
                            continue;
                        }
                        MetricThresholdEntry thresholdEntry = serverState.GetMetricThresholdEntry(ievent.MetricID);
                        if (thresholdEntry != null)
                        {
                            AdvancedAlertConfigurationSettings advancedSettings = thresholdEntry.Data as AdvancedAlertConfigurationSettings;
                            if (advancedSettings != null)
                            {
                                SnoozeInfo snoozeInfo = advancedSettings.SnoozeInfo;
                                if (snoozeInfo != null && snoozeInfo.IsSnoozed(serverTime.Value))
                                {
                                    LOG.DebugFormat("Skipping notification (snoozing {2} to {3}): [{0}]{1} ",
                                                    serverState.WrappedServer.InstanceName,
                                                    ievent,
                                                    snoozeInfo.StartSnoozing,
                                                    snoozeInfo.StopSnoozing);
                                    continue;
                                }
                            }
                        }
                        else continue;

                        MetricDefinition md = metricDefinitions.GetMetricDefinition(ievent.MetricID);
                        if (md != null)
                        {
                            Pair<int, IEvent> rank = new Pair<int, IEvent>(md.Rank, ievent);
                            filteredEvents.Add(rank);
                        }
                    }
                    if (filteredEvents.Count > 0)
                    {
                        // produce event list sorted by rank
                        filteredEvents.Sort(RankEvents);
                        List<IEvent> sortedEvents = new List<IEvent>(filteredEvents.Count);
                        foreach (Pair<int, IEvent> sfevent in filteredEvents)
                            sortedEvents.Add(sfevent.Second);

                        Process(sortedEvents, refresh);
                    }
                }
                else
                {
                    LOG.InfoFormat("Skipping notification for scheduled refresh: ({0} {1}) state graph not found.", refresh.ServerName, refresh.TimeStamp);
                }
            }
        }

        private static int RankEvents(Pair<int, IEvent> x, Pair<int, IEvent> y)
        {
            int rc = x.First.CompareTo(y.First);
            if (rc == 0)
            {
                rc = x.Second.MonitoredState.CompareTo(y.Second.MonitoredState);
                if (rc == 0)
                    rc = x.Second.MetricID.CompareTo(y.Second.MetricID);
            }
            return rc;
        }

        private IList<IEvent> GetUniqueEvents(IList<IEvent> ievents)
        {
            //Process ievents to remove duplicates
            List<IEvent> uniqueEvents = new List<IEvent>(ievents.Count);
            Dictionary<string, int> verifyDuplicity = new Dictionary<string, int>();
            string compoundKey = "";
            foreach (IEvent sfevent in ievents)
            {
                compoundKey = sfevent.OccuranceTime.ToString();
                compoundKey += sfevent.MonitoredObject.ToString();
                compoundKey += sfevent.MetricID;
                if (!verifyDuplicity.ContainsKey(compoundKey))
                {
                    verifyDuplicity.Add(compoundKey, sfevent.MetricID);
                    uniqueEvents.Add(sfevent);
                }
            }
            return uniqueEvents;
        }

        public void Process(IList<IEvent> ievents, AlertableSnapshot refresh)
        {
            List<NotificationContext> notifications = new List<NotificationContext>();
            if (ievents != null) ievents = GetUniqueEvents(ievents);
            else
            {
                LOG.ErrorFormat("ievents null - dropping notification");
                return;
            }

            using (LOG.InfoCall("Process"))
            {
                if (refresh == null)
                    LOG.DebugFormat("Processing {0} management service events", ievents.Count);
                else
                    LOG.DebugFormat("Processing {0} events: {1}", ievents.Count, refresh);

                if (ievents.Count == 0)
                    return;

                // make s local copy of the rules so that they won't change during evaluation
                sync.AcquireReaderLock(-1);
                NotificationRule[] rules = Collections.ToArray(ruleDictionary.Values as ICollection<NotificationRule>);
                sync.ReleaseReaderLock();

                if (rules == null)
                {
                    LOG.ErrorFormat("rules list is null - dropping notification");
                    return;
                }
                //MonitoredSqlServer server = (refresh != null) ? refresh.MonitoredServer : null;

                string serverInstanceName = (refresh != null) ? refresh.ServerName : null;
                // Initialize Processed IEvents
                IList<IEvent> processediEvents = ievents;
                List<int> serverTags = null;
                //if (server != null)
                //{
                MonitoredSqlServerState state = refresh != null ? Management.ScheduledCollection.GetCachedMonitoredSqlServer(refresh.Id) : null;
                if (state == null)
                {
                    if (refresh != null)
                    {
                        LOG.ErrorFormat("Cached state for server '{0}' found - dropping notification.", refresh.ServerName);
                    }
                    else
                    {
                        LOG.ErrorFormat("Refresh is null - dropping notification");
                    }
                    // Processed IEvents with Metric Id - SQLdmCollectionServiceStatus
                    processediEvents = ievents.Where(ievent => ievent.MetricID == (int)Metric.SQLdmCollectionServiceStatus).ToList();

                    if (processediEvents.Count == 0)
                    {
                        return;
                    }
                }
                else
                {
                    if (state.WrappedServer.HasTags)
                        serverTags = state.WrappedServer.Tags;
                }
                //}

                // Matched alerts for the notification rule will be saved in this list
                List<IEvent> matchedAlertEvents = new List<IEvent>();
                Int32 rankValue = 0;
                Int32 operand = 0;
                // iterate through all notification rules to see if there are any alerts that matches to all rule metrics

                foreach (NotificationRule rule in rules)
                {
                    if (!String.IsNullOrEmpty(rule.RankValue))
                    {
                        rankValueWithIndex = rule.RankValue.Trim().Split();
                        if (!string.IsNullOrEmpty(rankValueWithIndex[0]))
                        {
                            operand = Convert.ToInt32(rankValueWithIndex[0]);
                            rankValue = Convert.ToInt32(rankValueWithIndex[1]);
                        }
                    }

                    // The follwing if condition is for OR checkbox and the previous or related code is used here
                    if (!rule.IsMetricsWithAndChecked)
                    {
                        if (!rule.Enabled)
                            continue;

                        // does the rule match the server name or a server tag
                        if ((rule.Matches(serverInstanceName) || rule.Matches(serverTags)) == false)
                            continue;

                        // process the rules for each event
                        foreach (IEvent ievent in processediEvents)
                        {
                            // metricInfo = GetMetricInfo(MetricDefinition.GetMetric(ievent.MetricID));
                            MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();
                            metricdefinition = metricDefinitions.GetMetricDefinition(ievent.MetricID);

                            if (!rule.Matches(ievent.MetricID))
                                continue;

                            if (!rule.Matches(ievent.OccuranceTime.ToLocalTime()))
                                continue;

                            if (rule.StateChangeComparison.Enabled)
                            {
                                //DateTime finaltime = Convert.ToDateTime(DateTime.Now - ievent.OccuranceTime);
                                if (rule.Matches(ievent.MonitoredState))
                                {
                                    if (IsAlertRankAndSeverityConditions(metricdefinition.Rank, rule, rankValue, ievent, operand))
                                    {
                                        notifications.AddRange(ProcessNotifications(rule, refresh, ievent));
                                    }
                                }
                            }
                            else
                            {
                                if (ievent.MonitoredState > MonitoredState.OK)
                                {
                                    if (rule.StateComparison.Enabled)
                                    {
                                        if (rule.Matches(ievent.MonitoredState))
                                        {
                                            if (IsAlertRankAndSeverityConditions(metricdefinition.Rank, rule, rankValue, ievent, operand))
                                            {
                                                notifications.AddRange(ProcessNotifications(rule, refresh, ievent));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // The follwing else condition is for AND checkbox and the current code is used here
                    else
                    {
                        //clearing previously matched alerts
                        matchedAlertEvents.Clear();

                        if (!rule.Enabled)
                            continue;

                        // does the rule match the server name or a server tag
                        if ((rule.Matches(serverInstanceName) || rule.Matches(serverTags)) == false)
                            continue;

                        // This boolean parameter is to check if each metric of a notification rule is matched with atleast one alert metric.  
                        // If it becomes true then all metrics of a rule are associated with atleast one alert
                        Boolean blnIsAllMetricIdsPresent = true;

                        //SQLDM-30393.
                        if (rule.MetricIDs != null)
                        {
                            // process the rules for each event
                            foreach (int metricId in rule.MetricIDs)
                            {
                                Boolean blnRuleMetricIdPresent = false;
                                foreach (IEvent ievent in processediEvents)
                                {
                                    if (!rule.Matches(ievent.OccuranceTime.ToLocalTime()))
                                        continue;

                                    if (ievent.MetricID == metricId)
                                    {
                                        //// storing the alert details in the matchedAlertEvents list.
                                        matchedAlertEvents.Add(ievent);
                                        blnRuleMetricIdPresent = true;
                                        //break; SQLdm 10.0 (Gaurav Karwal): commented out to get all the alerts raised of a metric
                                    }
                                }
                                blnIsAllMetricIdsPresent = blnIsAllMetricIdsPresent & blnRuleMetricIdPresent;

                                // If one of the notification rule metric is not having any alert associated with the same metric id. 
                                // Then come out of the for loop to check another notification rule
                                if (!blnIsAllMetricIdsPresent)
                                    break;
                            }
                        }

                        if (blnIsAllMetricIdsPresent)
                        {
                            foreach (IEvent ievent in matchedAlertEvents)
                            {
                                if (rule.StateChangeComparison.Enabled)
                                {
                                    if (rule.Matches(ievent.MonitoredState))
                                    {
                                        if (IsAlertRankAndSeverityConditions(metricdefinition.Rank, rule, rankValue, ievent, operand))
                                        {
                                            notifications.AddRange(ProcessNotifications(rule, refresh, ievent));
                                        }
                                    }
                                }
                                else
                                {
                                    if (ievent.MonitoredState > MonitoredState.OK)
                                    {
                                        if (rule.StateComparison.Enabled)
                                        {
                                            if (rule.Matches(ievent.MonitoredState))
                                            {
                                                if (IsAlertRankAndSeverityConditions(metricdefinition.Rank, rule, rankValue, ievent, operand))
                                                {
                                                    notifications.AddRange(ProcessNotifications(rule, refresh, ievent));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (notifications.Count > 0)
                {
                    Management.ScheduledCollection.ScheduledCollectionQueues.EnqueueNotificationItems(notifications);
                }
            }
        }

        protected Boolean IsAlertRankAndSeverityConditions(Int32 metricRank, NotificationRule rule, Int32 rankValue, IEvent iEvent, Int32 operand)
        {
            DateTime occuranceDateTime = iEvent is StateDeviationUpdateEvent ? (iEvent as StateDeviationUpdateEvent).DeviationEvent.OccuranceTime.ToLocalTime() : iEvent is StateDeviationEvent ? (iEvent as StateDeviationEvent).OccuranceTime.ToLocalTime() : DateTime.Now.ToLocalTime();

            Boolean retVal = true;
            if (rule.IsRankValueChecked)
            {
                switch (operand)
                {
                    case 0:
                        retVal = (metricRank > rankValue); break;
                    case 1:
                        retVal = (metricRank < rankValue); break;
                    case 2:
                        retVal = (metricRank == rankValue); break;
                }
            }

            if (rule.IsMetricSeverityChecked && !iEvent.StateChanged) // && !string.IsNullOrWhiteSpace(rule.MetricSeverityValue))
            {
                double DiffTotalMinutes = (DateTime.Now - occuranceDateTime).TotalMinutes;
                retVal = rule.IsRankValueChecked ? retVal && DiffTotalMinutes >= Convert.ToDouble(rule.MetricSeverityValue) : DiffTotalMinutes >= Convert.ToDouble(rule.MetricSeverityValue);
            }
            else if (rule.StateChangeComparison != null && rule.StateChangeComparison.Enabled)
            {
                bool changed = !rule.IsMetricSeverityChecked && iEvent.StateChanged;
                retVal = rule.IsRankValueChecked ? retVal && changed : changed;
            }
            return retVal;
        }


        protected List<NotificationContext> ProcessNotifications(NotificationRule rule, AlertableSnapshot refresh, IEvent ievent)
        {
            List<NotificationContext> result = new List<NotificationContext>();
            try
            {
                NotificationDestinationInfo[] destinations =
                    Collections.ToArray<NotificationDestinationInfo>(
                        rule.Destinations as ICollection<NotificationDestinationInfo>);

                foreach (NotificationDestinationInfo destinationInfo in destinations)
                {
                    Guid destinationId = destinationInfo.ProviderID;
                    NotificationProviderContext providerContext = this[destinationId];

                    // only handle notifications for enabled providers
                    if (providerContext != null)
                    {
                        if (providerContext.IsEnabled && destinationInfo.Enabled)
                        {
                            try
                            {
                                NotificationContext notification =
                                    new NotificationContext(ievent, destinationInfo, rule, refresh);
                                result.Add(notification);
                            }
                            catch (Exception exception)
                            {
                                LOG.Error("Error sending notification", exception);
                            }
                        }
                        else
                        {
                            string providerName = providerContext.Provider.NotificationProviderInfo.Name;
                            if (destinationInfo.Enabled)
                            {
                                string message = String.Format("Action provider '{0}' for rule '{1}' is disabled",
                                    providerName,
                                    rule.Description);
                                LOG.Verbose(message);
                                AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                                      new MonitoredObjectName((string)null, (string)null),
                                                                      MonitoredState.Warning,
                                                                      message,
                                                                      message);
                            }
                            else
                                LOG.VerboseFormat("Action '{0}' for rule '{1}' is disabled", providerName, rule.Description);
                        }
                    }
                    else
                    {
                        LOG.ErrorFormat("Unable to locate notification provider for rule ({0}) having a provider id ({1})",
                                  rule.Description, destinationInfo.ProviderID);
                        string message = String.Format("Unable to locate notification provider for rule '{0}'", rule.Description);

                        AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                              new MonitoredObjectName((string)null, (string)null),
                                                              MonitoredState.Warning,
                                                              message,
                                                              message);
                    }
                }
            }
            catch (Exception exception)
            {
                LOG.Warn("Error processing notifications", exception);
            }

            return result;
        }

        internal void ChangeAlertConfiguration(AlertConfiguration configuration)
        {
            try
            {
                RepositoryHelper.ChangeAlertConfiguration(ManagementServiceConfiguration.ConnectionString, configuration);
                // TODO: Push the new configuration to the collection service for this instance
            }
            catch (Exception e)
            {
                throw new ManagementServiceException("Unable to update alert configuration.", e);
            }
        }

        internal void TryStartProvider(Guid destinationId)
        {
            NotificationProviderContext providerContext = null;
            if (contexts.TryGetValue(destinationId, out providerContext) && providerContext.IsEnabled)
            {
                providerContext.NotificationsAdded();
            }
        }
    }

    internal class NotificationProviderContext : IDisposable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationProviderContext");

        private INotificationProvider provider;
        private IQueue<NotificationContext> workQueue;
        private Timer retryTimer;
        private object sync = new object();

        private bool queueDrained = true;
        private int maxRetries = 4;         // TODO: take max retries from either provider info or global config
        private int retryInterval = 30;
        private int maxEventOccurrenceTimeInHrs = 8;

        internal NotificationProviderContext(NotificationProviderInfo providerInfo)
        {
            provider = providerInfo.CreateInstance();

            provider.SetEventLog(Management.EventLog);
            workQueue = Management.ScheduledCollection.ScheduledCollectionQueues.GetNotificationQueue(provider.NotificationProviderInfo.Id);
            // PersistenceManager.Instance.GetNotificationQueue(provider.NotificationProviderInfo.Id);
            retryTimer = new Timer(ProcessQueuedItems);

            // schedule background thread if the queue has items
            if (workQueue.Count > 0)
            {
                retryTimer.Change(0, Timeout.Infinite);
                queueDrained = false;
            }

        }

        public INotificationProvider Provider
        {
            get { return provider; }
        }

        public bool IsEnabled
        {
            get
            {
                bool result;
                lock (sync)
                {
                    result = provider.NotificationProviderInfo.Enabled;
                }
                return result;
            }
        }

        /// <summary>
        /// Notifications were added directly to the external queue and we need to start
        /// the timer to handle processing the items in the queue.
        /// </summary>
        public void NotificationsAdded()
        {
            lock (sync)
            {
                if (queueDrained && workQueue.Count > 0)
                {
                    retryTimer.Change(0, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Add a single notification and start the timer if necessary
        /// </summary>
        /// <param name="notification"></param>
        public void Add(NotificationContext notification)
        {
            lock (sync)
            {
                if (!provider.NotificationProviderInfo.Enabled)
                    return;

                workQueue.Enqueue(notification);

                if (workQueue.Count == 1)
                {
                    retryTimer.Change(0, Timeout.Infinite);
                }
            }
        }

        private void ProcessQueuedItems(object state)
        {
            using (LOG.VerboseCall("ProcessQueuedItems"))
            {
                lock (sync)
                {
                    if (workQueue.Count == 0)
                    {
                        LOG.Verbose("Unnecessary call - notification queue is empty.");
                        queueDrained = true;
                        return;
                    }
                    queueDrained = false;
                }

                Stopwatch stopwatch = new Stopwatch();

                for (; ; )
                {
                    NotificationContext context = null;
                    NotificationContext[] contexts = null;
                    try
                    {

                        if (provider is IBulkNotificationProvider)
                        {
                            contexts = workQueue.PeekAll();

                            if (contexts.Length > 0)
                            {
                                stopwatch.Start();
                                int numberProcessed = ((IBulkNotificationProvider)provider).Send(contexts);
                                stopwatch.Stop();

                                if (numberProcessed > 0)
                                {
                                    LOG.VerboseFormat("Bulk copy of {0} tasks took {1} ms.", numberProcessed, stopwatch.ElapsedMilliseconds);
                                    lock (sync)
                                    {
                                        // take the item off the queue
                                        workQueue.Dequeue(numberProcessed);
                                        if (workQueue.Count == 0)
                                        {
                                            queueDrained = true;
                                            return;
                                        }
                                    }
                                    if (numberProcessed == contexts.Length)
                                        continue;
                                }
                                LOG.WarnFormat(
                                    "Bulk copy of tasks did not process all queued messages ({0} of {1} processed).",
                                    numberProcessed,
                                    contexts.Length);
                            }
                        }

                        // get next item but leave on the queue
                        context = workQueue.Peek();
                        if (context != null)
                        {
                            if (context.SourceEvent != null && context.SourceEvent.OccuranceTime != null)
                            {
                                // If the event was raised before 8 hours, skip notification.
                                TimeSpan timediff = DateTime.Now.ToUniversalTime() - context.SourceEvent.OccuranceTime;
                                if (timediff.Hours < maxEventOccurrenceTimeInHrs)
                                {
                                    if (!provider.Send(context))
                                    {
                                        string message =
                                            String.Format("Notification provider error: {0}", context.LastSendException.Message);
                                        AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                                      new MonitoredObjectName((string)null, (string)null),
                                                                              MonitoredState.Warning,
                                                                              context.LastSendException == null
                                                                                  ? message
                                                                                  : context.LastSendException.Message,
                                                                              message);
                                    }
                                }
                                else
                                {
                                    LOG.Verbose("Source event is older than 8 hours, not sending this notification");
                                }
                            }
                            else
                            {
                                LOG.Error("Source event occurrence time is undefined, not sending this notification");
                            }
                        }
                        lock (sync)
                        {
                            // take the item off the queue
                            workQueue.Dequeue();
                            if (workQueue.Count == 0)
                            {
                                queueDrained = true;
                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //START-SQLdm 10.0 (Swati Gogia)-added to handle retry if exception occurs in case the provider is IBulkNotificationProvider
                        if (provider is IBulkNotificationProvider)
                        {
                            if (contexts != null)
                            {
                                foreach (NotificationContext notificationContext in contexts)
                                {
                                    ++notificationContext.SendAttempts; // increment send attempts
                                    if (notificationContext.SendAttempts >= maxRetries || workQueue.Count > 1000)
                                    {
                                        // remove item from queue
                                        workQueue.Dequeue();
                                        // write to log file
                                        LogAbandonedNotification(notificationContext);
                                    }
                                    else
                                    {
                                        LogAndRetryNotification(context, e);
                                        break;
                                    }
                                }
                            }
                        }
                        //END-SQLdm 10.0 (Swati Gogia)
                        if (context != null)
                        {
                            ++context.SendAttempts; // increment send attempts
                            if (context.SendAttempts >= maxRetries || workQueue.Count > 1000)
                            {
                                // remove item from queue
                                workQueue.Dequeue();
                                // write to log file
                                LogAbandonedNotification(context);
                            }
                            else
                            {
                                LogAndRetryNotification(context, e);
                                break;
                            }
                        }
                        else
                            LOG.Error(e);
                    }
                }
            }
        }

        public void LogAndRetryNotification(NotificationContext context, Exception e)
        {
            String message = String.Format(
                    "Notification provider {0} encountered an error during a send.  Retry in {1} second(s).",
                    provider.NotificationProviderInfo.Name, retryInterval);
            LOG.Error(message, e);

            retryTimer.Change(retryInterval * 1000, Timeout.Infinite);
        }


        public void LogAbandonedNotification(NotificationContext context)
        {
            String message = String.Format("Notification abandoned after {0} retries for provider {1}.  Provider queue length is {2}.",
                                           context.SendAttempts,
                                           provider.NotificationProviderInfo.Name,
                                           workQueue.Count);

            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                      new MonitoredObjectName((string)null, (string)null),
                                      MonitoredState.Warning,
                                      context.LastSendException == null ? message : context.LastSendException.Message,
                                      message);

            LOG.Error(message, context.LastSendException);
        }

        //        public void Start()
        //        {
        //            workQueue.Start();
        //        }

        public void UpdateNotificationProvider(NotificationProviderInfo providerInfo)
        {
            lock (sync)
            {
                provider.NotificationProviderInfo = providerInfo;
                provider.SetEventLog(Management.EventLog);
            }
        }

        public void Dispose()
        {
            retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
