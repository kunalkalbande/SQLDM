//------------------------------------------------------------------------------
// <copyright file="EnablePANotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Diagnostics;
    using Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Analysis;

    /// <summary>
    /// Notification Provider for enabline the query monitor.
    /// </summary>
    public class EnablePANotificationProvider : INotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("EnablePANotificationProvider");
        private EventLogNotificationProviderInfo info;
        private EventLog eventLog;
        
        private object sync = new object();

        public EnablePANotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                //MonitoredSqlServer collectionServer = context.Refresh.MonitoredServer;
                if (context.Refresh != null)
                {
                    EnablePADestination destination = context.Destination as EnablePADestination;
                    MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(context.Refresh.Id);
                    bool alreadyEnabled = false;
                    if (state.WrappedServer.AnalysisConfiguration != null)
                    {
                        //To Add categories which are selected by Response alert UI
                        AnalysisConfiguration clonedCopyAnalysisConfiguration = ObjectHelper.Clone(state.WrappedServer.AnalysisConfiguration);
                        List<int> blcokedCategories = new List<int>();
                        Dictionary<int, string> blockedList = new Dictionary<int, string>();
                        foreach (RecommendationCategory item in destination.BlockedCategoriesListObject)
                        {
                            blcokedCategories.Add(item.CategoryId);
                            blockedList.Add(item.CategoryId, item.CategoryName);
                        }
                        clonedCopyAnalysisConfiguration.BlockedCategoryID = blcokedCategories;
                        clonedCopyAnalysisConfiguration.BlockedCategories = blockedList;
                        ManagementService mgmtService = new ManagementService();
                        mgmtService.GetPrescriptiveAnalysisResult(context.Refresh.Id, clonedCopyAnalysisConfiguration, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType.AlertDefault);
                                        alreadyEnabled = true;
                    }

                    if (alreadyEnabled)
                    {
                        LOG.InfoFormat("Prescriptive Analyzer already enabled for '{0}'", context.Refresh.ServerName);
                        return true;
                    }

                 }
                return true;
            }
        }

        private void LogAndAddAlert(string message, Exception e)
        {
            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                new MonitoredObjectName((string)null, (string)null),
                                e != null ? MonitoredState.Warning : MonitoredState.OK,
                                message,
                                (e == null) ? message : message + "[" + e.Message + "]");

            if (e == null)
                LOG.Info(message);
            else
                LOG.Error(message, e);
        }

        public NotificationProviderInfo NotificationProviderInfo
        {
            get
            {
                lock (sync)
                {
                    return this.info;
                }
            }

            set
            {
                lock (sync)
                {
                    string operation = this.info == null ? "created" : "updated";

                    if (value is EventLogNotificationProviderInfo)
                        this.info = value as EventLogNotificationProviderInfo;
                    else
                    {
                        this.info = new EventLogNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Enable Prescriptive Analyzer notification provider {0}", operation);
                }
            }
        }


    }
}
