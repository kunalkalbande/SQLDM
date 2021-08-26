//------------------------------------------------------------------------------
// <copyright file="SmtpNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Notification;

namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using Idera.SQLdm.Common.Events;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Notification.Providers;
    using SnmpSharpNet;
    using Idera.SQLdm.Common;
    using Monitoring;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    enum Platform
    {
        SQLServer = 1,
        Sharepoint
    }

    enum Products
    {
        SQLdr = 1,
        SQLdm,
        VDB,
        SQLsafe,
        SPdm
    }

    enum dmVars
    {
        AlertSummary = 1,
        AlertText,
        Metric,
        Severity,
        Instance,
        Database,
        Table,
        Timestamp,
        Value,
        Description,
        Comments,
        Hostname    // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
    }

    enum alerts
    {
        SQLdmTrap = 0,
        SQLdmTrapTest = 99
    }

    /// <summary>
    /// SNMP Notification Provider for SQLdm Management Service
    /// </summary>
    public class SnmpNotificationProvider : INotificationProvider, IBulkNotificationProvider
    {
        #region fields
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SnmpNotificationProvider");

        private SnmpNotificationProviderInfo info;
        private object sync = new object();
        private EventLog eventLog;
        private const string IderaOID = "1.3.6.1.4.1.24117";

        #endregion

        #region constructors

        public SnmpNotificationProvider()
        {
        }

        public SnmpNotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
        }

        #endregion

        #region properties

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

                    if (value is SnmpNotificationProviderInfo)
                        this.info = value as SnmpNotificationProviderInfo;
                    else
                    {
                        this.info = new SnmpNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Snmp notification provider {0}: {1}", operation, this.info.Address);
                }
            }
        }

        #endregion

        #region methods

        public bool Send(NotificationContext context)
        {
            SnmpDestination destination = null;
            int TrapID;

            VbCollection vbc = new VbCollection();

            try
            {
                string varText = "";
                Oid varOID = new Oid();

                destination = (SnmpDestination)context.Destination;
                IEvent baseEvent = context.SourceEvent;

                if (baseEvent != null)
                {
                    MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;

                    if (baseEvent.AdditionalData is CustomCounterSnapshot)
                    {
                        // add in the metric description info to the additional 
                        // data if this is a custom counter snapshot
                        baseEvent.AdditionalData =
                            new Pair<CustomCounterSnapshot, MetricDescription?>(
                                (CustomCounterSnapshot)baseEvent.AdditionalData,
                                metricDefinitions.GetMetricDescription(baseEvent.MetricID));

                    }

                    //                    messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                    // Alert Summary
                    varText = NotificationMessageFormatter.FormatMessage("$(AlertSummary)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - AlertSummary: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Alert Text
                    varText = NotificationMessageFormatter.FormatMessage("$(AlertText)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertText));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - AlertText: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Severity
                    varText = NotificationMessageFormatter.FormatMessage("$(Severity)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Severity));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Severity: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Instance
                    varText = NotificationMessageFormatter.FormatMessage("$(Instance)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Instance));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Instance: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Database
                    varText = NotificationMessageFormatter.FormatMessage("$(Database)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Database));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Database: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Table
                    varText = NotificationMessageFormatter.FormatMessage("$(Table)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Table));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Table: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Metric
                    varText = NotificationMessageFormatter.FormatMessage("$(Metric)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Metric));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Metric: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Description
                    varText = NotificationMessageFormatter.FormatMessage("$(Description)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Description));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Description: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Value
                    varText = NotificationMessageFormatter.FormatMessage("$(Value)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Value));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Value: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Timestamp
                    varText = NotificationMessageFormatter.FormatMessage("$(Timestamp)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Timestamp));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Timestamp: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // Comments
                    varText = NotificationMessageFormatter.FormatMessage("$(Comments)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Comments));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Comments: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                    // Hostname
                    varText = NotificationMessageFormatter.FormatMessage("$(Hostname)", context.Refresh, baseEvent, false);
                    varText = replaceNonPrintableChars(varText);
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Hostname));
                    vbc.Add(varOID, new OctetString(varText));
                    LOG.DebugFormat("SNMP Notification - Hostname: [{0}]", varText);
                    varText = "";
                    varOID.Reset();

                    TrapID = (int)alerts.SQLdmTrap;

                } // end if (baseEvent != null)
                else // if (baseEvent == null) this must be a test message
                {
                    // Setup test message
                    varText = "This is a test SNMP Trap from the SQLDM product.";
                    varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                    vbc.Add(varOID, new OctetString(varText));
                    varText = "";
                    varOID = null;

                    TrapID = (int)alerts.SQLdmTrapTest;
                } // end test message
            } // end try
            catch (Exception e)
            {
                context.lastSendException = e;
                LOG.Error("SNMP Notification - Failed constructing/sending SNMP Trap message", e);
                return false;
            }

            try
            {
                IpAddress ipAddress = new IpAddress(Dns.GetHostName());
                TrapAgent agent = new TrapAgent();

                LOG.DebugFormat("Sending SNMP Trap message to {0}:{1} from  ip {2}", info.Address, info.Port.ToString(), ipAddress.ToString());

                agent.SendV1Trap(new IpAddress(info.Address), info.Port, info.Community,
                    new Oid(formatEnterpriseOID(Products.SQLdm)), ipAddress, 6, TrapID, 0, vbc);

                LOG.Debug("SNMP Trap sent");
            }
            catch (Exception e)
            {
                context.LastSendException = e;
                LOG.Error("SNMP Notification - Failed sending the Trap Message", e);
                return false;
            }

            return true;
        }

        
        //START SQLdm 10.0 (Swati Gogia)-- added code to concatenate body of different events for multi-metric alert to send a consolidated mail
        public int Send(NotificationContext[] notifications)
        {
            SnmpDestination destination = null;
            int TrapID=0;

            VbCollection vbc = new VbCollection();
            List<NotificationContext> allContexts = new List<NotificationContext>(notifications);
            List<NotificationContext> combinedNotificationContextList = null;
            List<NotificationContext> atomicNotificationContextList = null;
            Guid ruleId = Guid.NewGuid();
            int numberOfProcessedContexts = 0;
            IEvent baseEvent = null;
            string varText = "";
            Oid varOID = null;
            try
            {
                combinedNotificationContextList = allContexts.FindAll(x => x.Rule.IsMetricsWithAndChecked == true);
                //sorting on the rule id
                combinedNotificationContextList.Sort(delegate(NotificationContext n1, NotificationContext n2)
                {
                    return n1.Rule.Id.CompareTo(n2.Rule.Id);
                });

                atomicNotificationContextList = allContexts.FindAll(x => x.Rule.IsMetricsWithAndChecked == false);

                
                if (combinedNotificationContextList.Count > 0)
                {
                    ruleId = combinedNotificationContextList[0].Rule.Id;            //SQLdm 10.0.2 (Barkha Khatri) added this line inside if to avoid indexOutOfRangeException
                    varOID = new Oid();
                    foreach (NotificationContext context in combinedNotificationContextList)
                    {
                        numberOfProcessedContexts++;
                        if (context.Rule.Id.Equals(ruleId))
                        {
                            varText = "";
                            
                            destination = (SnmpDestination)context.Destination;
                            baseEvent = context.SourceEvent;

                            if (baseEvent != null)
                            {
                                MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;

                                if (baseEvent.AdditionalData is CustomCounterSnapshot)
                                {
                                    // add in the metric description info to the additional 
                                    // data if this is a custom counter snapshot
                                    baseEvent.AdditionalData =
                                        new Pair<CustomCounterSnapshot, MetricDescription?>(
                                            (CustomCounterSnapshot)baseEvent.AdditionalData,
                                            metricDefinitions.GetMetricDescription(baseEvent.MetricID));

                                }

                                //                    messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                                // Alert Summary
                                varText = NotificationMessageFormatter.FormatMessage("$(AlertSummary)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - AlertSummary: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Alert Text
                                varText = NotificationMessageFormatter.FormatMessage("$(AlertText)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertText));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - AlertText: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Severity
                                varText = NotificationMessageFormatter.FormatMessage("$(Severity)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Severity));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Severity: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Instance
                                varText = NotificationMessageFormatter.FormatMessage("$(Instance)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Instance));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Instance: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Database
                                varText = NotificationMessageFormatter.FormatMessage("$(Database)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Database));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Database: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Table
                                varText = NotificationMessageFormatter.FormatMessage("$(Table)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Table));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Table: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Metric
                                varText = NotificationMessageFormatter.FormatMessage("$(Metric)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Metric));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Metric: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Description
                                varText = NotificationMessageFormatter.FormatMessage("$(Description)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Description));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Description: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Value
                                varText = NotificationMessageFormatter.FormatMessage("$(Value)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Value));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Value: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Timestamp
                                varText = NotificationMessageFormatter.FormatMessage("$(Timestamp)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Timestamp));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Timestamp: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Comments
                                varText = NotificationMessageFormatter.FormatMessage("$(Comments)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Comments));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Comments: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                                // Hostname
                                varText = NotificationMessageFormatter.FormatMessage("$(Hostname)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Hostname));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Hostname: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                TrapID = (int)alerts.SQLdmTrap;

                            } // end if (baseEvent != null)
                            else // if (baseEvent == null) this must be a test message
                            {
                                // Setup test message
                                varText = "This is a test SNMP Trap from the SQLDM product.";
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                                vbc.Add(varOID, new OctetString(varText));
                                varText = "";
                                varOID = null;

                                TrapID = (int)alerts.SQLdmTrapTest;
                            }
                        }
                        else
                        {
                            try
                            {
                                IpAddress ipAddress = new IpAddress(Dns.GetHostName());
                                TrapAgent agent = new TrapAgent();

                                LOG.DebugFormat("Sending SNMP Trap message to {0}:{1} from  ip {2}", info.Address, info.Port.ToString(), ipAddress.ToString());

                                agent.SendV1Trap(new IpAddress(info.Address), info.Port, info.Community,
                                    new Oid(formatEnterpriseOID(Products.SQLdm)), ipAddress, 6, TrapID, 0, vbc);

                                LOG.Debug("SNMP Trap sent");
                            }
                            catch (Exception e)
                            {
                                // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap
                                if((combinedNotificationContextList.Count > 0) && (numberOfProcessedContexts - 1 < combinedNotificationContextList.Count))
                                {
                                    combinedNotificationContextList[numberOfProcessedContexts-1].LastSendException = e;
                                }
                                LOG.Error("SNMP Notification - Failed sending the Trap Message", e);

                                NotifySNMPMessageFailure(numberOfProcessedContexts, allContexts.Count);
                                // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap

                            }

                            varOID = new Oid();
                            varText = "";
                            vbc = new VbCollection();
                            destination = (SnmpDestination)context.Destination;
                            baseEvent = context.SourceEvent;
                            if (baseEvent != null)
                            {
                                MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;

                                if (baseEvent.AdditionalData is CustomCounterSnapshot)
                                {
                                    // add in the metric description info to the additional 
                                    // data if this is a custom counter snapshot
                                    baseEvent.AdditionalData =
                                        new Pair<CustomCounterSnapshot, MetricDescription?>(
                                            (CustomCounterSnapshot)baseEvent.AdditionalData,
                                            metricDefinitions.GetMetricDescription(baseEvent.MetricID));

                                }

                                //                    messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                                // Alert Summary
                                varText = NotificationMessageFormatter.FormatMessage("$(AlertSummary)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - AlertSummary: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Alert Text
                                varText = NotificationMessageFormatter.FormatMessage("$(AlertText)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertText));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - AlertText: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Severity
                                varText = NotificationMessageFormatter.FormatMessage("$(Severity)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Severity));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Severity: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Instance
                                varText = NotificationMessageFormatter.FormatMessage("$(Instance)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Instance));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Instance: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Database
                                varText = NotificationMessageFormatter.FormatMessage("$(Database)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Database));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Database: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Table
                                varText = NotificationMessageFormatter.FormatMessage("$(Table)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Table));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Table: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Metric
                                varText = NotificationMessageFormatter.FormatMessage("$(Metric)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Metric));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Metric: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Description
                                varText = NotificationMessageFormatter.FormatMessage("$(Description)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Description));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Description: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Value
                                varText = NotificationMessageFormatter.FormatMessage("$(Value)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Value));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Value: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Timestamp
                                varText = NotificationMessageFormatter.FormatMessage("$(Timestamp)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Timestamp));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Timestamp: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // Comments
                                varText = NotificationMessageFormatter.FormatMessage("$(Comments)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Comments));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Comments: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                                // Hostname
                                varText = NotificationMessageFormatter.FormatMessage("$(Hostname)", context.Refresh, baseEvent, false);
                                varText = replaceNonPrintableChars(varText);
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.Hostname));
                                vbc.Add(varOID, new OctetString(varText));
                                LOG.DebugFormat("SNMP Notification - Hostname: [{0}]", varText);
                                varText = "";
                                varOID.Reset();

                                TrapID = (int)alerts.SQLdmTrap;

                            } // end if (baseEvent != null)
                            else // if (baseEvent == null) this must be a test message
                            {
                                // Setup test message
                                varText = "This is a test SNMP Trap from the SQLDM product.";
                                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                                vbc.Add(varOID, new OctetString(varText));
                                varText = "";
                                varOID = null;

                                TrapID = (int)alerts.SQLdmTrapTest;
                            }

                        }
                    }

                    try
                    {
                        IpAddress ipAddress = new IpAddress(Dns.GetHostName());
                        TrapAgent agent = new TrapAgent();

                        LOG.DebugFormat("Sending SNMP Trap message to {0}:{1} from  ip {2}", info.Address, info.Port.ToString(), ipAddress.ToString());

                        agent.SendV1Trap(new IpAddress(info.Address), info.Port, info.Community,
                            new Oid(formatEnterpriseOID(Products.SQLdm)), ipAddress, 6, TrapID, 0, vbc);

                        LOG.Debug("SNMP Trap sent");
                    }
                    catch (Exception e)
                    {
                        if (combinedNotificationContextList.Count > 0)      //SQLdm 10.0.2 (Barkha Khatri) check for empty list
                        {
                        combinedNotificationContextList[combinedNotificationContextList.Count-1].LastSendException = e;
                        }
                        LOG.Error("SNMP Notification - Failed sending the Trap Message", e);

                        // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap
                        NotifySNMPMessageFailure(numberOfProcessedContexts, allContexts.Count);
                    }
                }

                if (atomicNotificationContextList.Count > 0)
                {
                    foreach (NotificationContext context in atomicNotificationContextList)
                    {
                        numberOfProcessedContexts++;
                        if (context != null)
                        {
                            try
                            {
                                SendContext(context);
                            }
                            catch (Exception e)
                            {
                                context.LastSendException = e;
                                LOG.Error("SNMP Notification - Failed sending the Trap Message", e);

                                // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap
                                NotifySNMPMessageFailure(numberOfProcessedContexts, atomicNotificationContextList.Count);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
               // context.lastSendException = e;
                LOG.Error("SNMP Notification - Failed constructing/sending SNMP Trap message", e);
                
                // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap
                NotifySNMPMessageFailure(numberOfProcessedContexts, allContexts.Count);
            }

            return allContexts.Count;
        }

        // SQLDM-26707 - Large volumes of SQLdm alerts overload the SNMP trap
        /// <summary>
        /// Notifies the user about SNMP Failure details by sending a SNMP message
        /// </summary>
        /// <param name="numberOfProcessedContexts">Number of context processed</param>
        /// <param name="allContextsCount">Total number of contexts</param>
        private void NotifySNMPMessageFailure(int numberOfProcessedContexts, int allContextsCount)
        {
            try
            {
                var varOID = new Oid();
                var vbc = new VbCollection();
                var varText = string.Format("At {0}, {1} SNMP Traps were sent. {2} were not sent. Please check the alerts view in SQL Diagnostic Manager.", DateTime.Now,
                    numberOfProcessedContexts, allContextsCount - numberOfProcessedContexts);
                varText = replaceNonPrintableChars(varText);
                varOID.Set(formatBindingsOID(Platform.SQLServer, Products.SQLdm, dmVars.AlertSummary));
                vbc.Add(varOID, new OctetString(varText));
                IpAddress ipAddress = new IpAddress(Dns.GetHostName());
                TrapAgent agent = new TrapAgent();


                agent.SendV1Trap(new IpAddress(info.Address), info.Port, info.Community,
                    new Oid(formatEnterpriseOID(Products.SQLdm)), ipAddress, 6, (int)alerts.SQLdmTrap, 0, vbc);

                LOG.Debug("SNMP Trap for NotifySNMPMessageFailure sent");
            }
            catch (Exception exception)
            {
                LOG.Error("SNMP Notification - Error in NotifySNMPMessageFailure method", exception);
            }
        }

        //calls Send function with a Notification Context to send an Smtp message.
        void SendContext(NotificationContext context)
        {

            int maxEventOccurrenceTimeInHrs = 8;
            try
            {
                if (context.SourceEvent != null && context.SourceEvent.OccuranceTime != null)
                {
                    // If the event was raised before 8 hours, skip notification.
                    TimeSpan timediff = DateTime.Now.ToUniversalTime() - context.SourceEvent.OccuranceTime;
                    if (timediff.Hours < maxEventOccurrenceTimeInHrs)
                    {
                        if (!Send(context))
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
            catch (Exception e)
            {
                throw e;
            }
        }
        //end SQLdm 10.0 (Swati Gogia)


        private static string formatEnterpriseOID(Products product)
        {
            return formatEnterpriseOID((int)product);
        }

        private static string formatEnterpriseOID(int product)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(IderaOID);

            switch (product)
            {
                case (int)Products.SQLdm:
                case (int)Products.SQLdr:
                case (int)Products.SQLsafe:
                case (int)Products.VDB:
                    sb.Append("." + ((int)Platform.SQLServer).ToString());
                    break;
                case (int)Products.SPdm:
                    sb.Append("." + ((int)Platform.Sharepoint).ToString());
                    break;
            }
            sb.Append("." + product.ToString());

            return sb.ToString();
        }

        private static string formatBindingsOID(Platform prodLine, Products product, dmVars var)
        {
            return formatBindingsOID((int)prodLine, (int)product, (int)var);
        }

        private static string formatBindingsOID(int prodLine, int product, int var)
        {
            return (IderaOID + "." + prodLine.ToString() + "." + product.ToString() + "." + var.ToString());
        }

        private static string replaceNonPrintableChars(string varText)
        {
            StringBuilder sb = new StringBuilder();
            char r = '?';
            foreach (char c in varText)
            {
                // If c is not a printable character replace it with a '?'
                sb.Append(((c > 31 && c < 127) || (c == 10) || (c == 13)) ? c : r);
            }
            return sb.ToString();
        }

        private void LogAndAddAlert(string message, Exception e)
        {
            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                new MonitoredObjectName((string)null, (string)null),
                                MonitoredState.Warning,
                                message,
                                (e == null) ? message : message + "[" + e.Message + "]");

            LOG.Error(message, e);
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        #endregion
    }
}
